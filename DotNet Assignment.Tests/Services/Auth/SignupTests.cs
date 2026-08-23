using DotNet_Assignment.Constants;
using DotNet_Assignment.Data;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Repository.RefreshTokens;
using DotNet_Assignment.Repository.Users;
using DotNet_Assignment.Services.Auth;
using DotNet_Assignment.Services.JWT;
using DotNet_Assignment.Tests.Utils;
using DotNet_Assignment.Utils;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace DotNet_Assignment.Tests.Services.Auth
{
    [TestFixture]
    public class SignupTests
    {
        private Mock<IUserRepository> _userRepository;
        private JWTService _jWTService;
        private Mock<IRefreshTokenRepository> _refreshTokenRepository;
        private Mock<AppDbContext> _appDbContext;
        private AuthService _authService;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _jWTService = new JWTService();
            _refreshTokenRepository = new Mock<IRefreshTokenRepository>();
            _appDbContext = new Mock<AppDbContext>();

            _authService = new AuthService(
                _userRepository.Object,
                _jWTService,
                _refreshTokenRepository.Object,
                _appDbContext.Object
            );
        }

        /// <summary>
        /// Register function - email already exists - throws exception
        /// </summary>
        [Test]
        public async Task Register_EmailAlreadyExists_ThrowsException()
        {
            var requestDto = new SignupRequestDto
            {
                Email = "unittest@gmail.com"
            };

            _userRepository.
                Setup(x => x.FindUserByEmailAsync(requestDto.Email)).
                ReturnsAsync(true);

            Func<Task> action = async() => await _authService.RegisterAsync(requestDto);

            var exception= Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.EmailAlreadyExists));
            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        /// <summary>
        /// Register function - valid user - hashes password
        /// </summary>
        [Test]
        public async Task Register_ValidUser_HashesPassword()
        {
            var requestDto = AuthTestUtil.CreateMockSignupRequestDto();

            ValidSignupHelper(requestDto);

            Func<Task> action = async () => await _authService.RegisterAsync(requestDto);

            Assert.DoesNotThrowAsync(action);

            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// Register function - valid user - adds user
        /// </summary>
        [Test]
        public async Task Register_ValidDeails_AddsUser()
        {
            var requestDto = AuthTestUtil.CreateMockSignupRequestDto();

            ValidSignupHelper(requestDto);

            Func<Task> action = async () => await _authService.RegisterAsync(requestDto);

            Assert.DoesNotThrowAsync(action);

            _userRepository.Verify(x => x.AddUser(It.IsAny<User>()),Times.Once);
            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// Register function - valid user - generate tokens
        /// </summary>
        [Test]
        public async Task Register_ValidUser_GenerateTokens()
        {
            var requestDto = AuthTestUtil.CreateMockSignupRequestDto();

            ValidSignupHelper(requestDto);

            JWTResponseDto response = null;
            Func<Task> action = async () => response = await _authService.RegisterAsync(requestDto);

            Assert.DoesNotThrowAsync(action);
            Assert.That(response.AccessToken, Is.Not.Null);

            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// Register function - valid user - adds refresh token
        /// </summary>
        [Test]
        public async Task Register_ValidUser_AddsRefreshToken()
        {
            var requestDto = AuthTestUtil.CreateMockSignupRequestDto();

            ValidSignupHelper(requestDto);

            Func<Task> action = async () => await _authService.RegisterAsync(requestDto);

            Assert.DoesNotThrowAsync(action);

            _refreshTokenRepository.Verify(x => x.AddRefreshToken(It.IsAny<RefreshToken>()),Times.Once);
            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// Register function - valid user - saves changes
        /// </summary>
        [Test]
        public async Task Register_SavesChanges()
        {
            var requestDto = AuthTestUtil.CreateMockSignupRequestDto();

            ValidSignupHelper(requestDto);

            Func<Task> action = async () => await _authService.RegisterAsync(requestDto);

            Assert.DoesNotThrowAsync(action);

            _appDbContext.Verify(x => x.SaveChangesAsync(),Times.Once);
        }

        /// <summary>
        /// Register function - valid user - returns jwt response - access token and refresh token
        /// </summary>
        [Test]
        public async Task Register_ValidUser_ReturnTokens()
        {
            var requestDto = AuthTestUtil.CreateMockSignupRequestDto();

            ValidSignupHelper(requestDto);

            var response = await _authService.RegisterAsync(requestDto);

            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);

        }

        /// <summary>
        /// Signup Helper - mocks signup flow
        /// </summary>
        /// <param name="RequestDto"></param>
        /// <returns>user</returns>
        private User ValidSignupHelper(SignupRequestDto RequestDto)
        {
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = RequestDto.Email,
                Password = Hasher.Hash(RequestDto.Password),
                IsDeleted = false
            };

            _userRepository
                .Setup(x => x.GetUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            _jWTService.GetAccessToken(user);

            _jWTService.GenerateRefreshToken();

            return user;
        }
    }
}
