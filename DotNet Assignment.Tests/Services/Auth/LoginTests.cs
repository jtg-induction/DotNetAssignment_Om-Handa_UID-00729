using DotNet_Assignment.Repository.RefreshTokens;
using DotNet_Assignment.Repository.Users;
using DotNet_Assignment.Services.Auth;
using DotNet_Assignment.Services.JWT;
using DotNet_Assignment.Tests.Utils;
using DotNet_Assignment.Models.Entities;
using Moq;
using NUnit.Framework;
using System;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Utils;
using DotNet_Assignment.Data;
using System.Threading.Tasks;
using DotNet_Assignment.Constants;
using DotNet_Assignment.Tests.Constants;

namespace DotNet_Assignment.Tests.Services.Auth
{
    [TestFixture]
    public class LoginTests
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
        /// Login Function - User Does not exist - throws Exception
        /// </summary>
        [Test]
        public async Task Login_UserDoesNotExist_ThrowsInvalidCredentialsException()
        {
            var request = AuthTestUtil.CreateMockLoginRequestDto();

            _userRepository
                .Setup(x => x.GetUserByEmailAsync(request.Email))
                .ReturnsAsync((User)null);

            Func<Task> action = async () => await _authService.LoginAsync(request);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.InvalidCredentials));

            _appDbContext.Verify( x=> x.SaveChanges(), Times.Never);
        }

        /// <summary>
        /// Login Function - User Deactivated - throws Exception
        /// </summary>
        [Test]
        public async Task Login_UserIsDeactivated_ThrowsUserDeactivatedException()
        {
            var request = AuthTestUtil.CreateMockLoginRequestDto();

            var user = new User
            {
                Email = request.Email,
                Password = MockConstants.MockHashedPassword,
                IsDeleted = true
            };

            _userRepository
                .Setup(x => x.GetUserByEmailAsync(request.Email))
                .ReturnsAsync(user);

            Func<Task> action = async () => await _authService.LoginAsync(request);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.UserDeactivated));

            _appDbContext.Verify( x=> x.SaveChanges(), Times.Never);
        }

        /// <summary>
        /// Login Function - Invalid Password - throws Exception
        /// </summary>
        [Test]
        public async Task Login_InvalidPassword_ThrowsInvalidCredentialsException()
        {
            var request = AuthTestUtil.CreateMockLoginRequestDto();

            var user = new User
            {
                Email = request.Email,
                Password = Hasher.Hash(Constants.MockConstants.MockPassword),    
                IsDeleted = false
            };

            _userRepository
                .Setup(x => x.GetUserByEmailAsync(request.Email))
                .ReturnsAsync(user);

            Func<Task> action = async () => await _authService.LoginAsync(request);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.InvalidCredentials));

            _appDbContext.Verify(x => x.SaveChanges(), Times.Never);
        }

        /// <summary>
        /// Login Function - Valid Credentials - Returns JWT response - Access and refresh token
        /// </summary>
        [Test]
        public async Task Login_ValidCredentials_ReturnsAccessAndRefreshTokens()
        {
            var request = AuthTestUtil.CreateMockLoginRequestDto();

            ValidLoginHelper(request);

            var response = await _authService.LoginAsync(request);

            Func<Task> action = async () => await _authService.LoginAsync(request);

            Assert.DoesNotThrowAsync(action);
        }


        /// <summary>
        /// Login Function -Valid Credentials - Password verified
        /// </summary>
        [Test]
        public async Task Login_ValidCredentials_VerifiesPassword()
        {
            var request = AuthTestUtil.CreateMockLoginRequestDto();

            var User = ValidLoginHelper(request);

            Func<Task> action = async () => await _authService.LoginAsync(request);

            Assert.DoesNotThrowAsync(action);

            Hasher.Verify(request.Password, User.Password);

            _appDbContext.Verify(x => x.SaveChanges(), Times.Once);
        }

        /// <summary>
        /// Login Function - Valid Credentials - Generates Access and refresh token
        /// </summary>
        [Test]
        public async Task Login_ValidCredentials_GenerateTokens()
        {
            var request = AuthTestUtil.CreateMockLoginRequestDto();

            ValidLoginHelper(request);

            JWTResponseDto response = null;
            Func<Task> action = async () => response = await _authService.LoginAsync(request);

            Assert.DoesNotThrowAsync(action);
            Assert.That(response.AccessToken, Is.Not.Null);

            _appDbContext.Verify(x => x.SaveChanges(), Times.Once);
        }

        /// <summary>
        /// Login Function - Valid Credentials - Adds refresh token
        /// </summary>
        [Test]
        public async Task Login_ValidCredentials_AddsRefreshToken()
        {
            
            var request = AuthTestUtil.CreateMockLoginRequestDto();

            ValidLoginHelper(request);

            Func<Task> action = async () => await _authService.LoginAsync(request);

            Assert.DoesNotThrowAsync(action);

            _refreshTokenRepository.Verify(
                x => x.AddRefreshToken(It.IsAny<RefreshToken>()),
                Times.Once);
            _appDbContext.Verify(x => x.SaveChanges(), Times.Once);
        }

        /// <summary>
        /// Login Function - Valid Credentials - Saves refresh token
        /// </summary>
        [Test]
        public async Task Login_ValidCredentials_SavesRefreshToken()
        {
            
            var request = AuthTestUtil.CreateMockLoginRequestDto();

            ValidLoginHelper(request);

            Func<Task> action = async () => await _authService.LoginAsync(request);

            Assert.DoesNotThrowAsync(action);
            _appDbContext.Verify(
                x => x.SaveChanges(),
                Times.Once);
        }

        /// <summary>
        /// Login Helper- mocking login flow
        /// </summary>
        /// <param name="RequestDto">Login request details</param>
        /// <returns>user</returns>
        private User ValidLoginHelper(LoginRequestDto RequestDto)
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
