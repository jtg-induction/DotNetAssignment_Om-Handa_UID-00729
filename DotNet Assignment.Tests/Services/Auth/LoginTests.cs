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

namespace DotNet_Assignment.Tests.Services.Auth
{
    [TestFixture]
    public class LoginTests
    {

        private Mock<IUserRepository> _userRepository;
        private Mock<IJWTService> _jWTService;
        private Mock<IRefreshTokenRepository> _refreshTokenRepository;
        private Mock<AppDbContext> _appDbContext;
        private AuthService _authService;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _jWTService = new Mock<IJWTService>();
            _refreshTokenRepository = new Mock<IRefreshTokenRepository>();
            _appDbContext = new Mock<AppDbContext>();

            _authService = new AuthService(
                _userRepository.Object,
                _jWTService.Object,
                _refreshTokenRepository.Object,
                _appDbContext.Object
            );
        }

        [Test]
        public async Task Login_UserDoesNotExist_ThrowsInvalidCredentialsException()
        {
            var Request = AuthTestUtil.CreateMockLoginRequestDto();

            _userRepository
                .Setup(x => x.GetUserByEmailAsync(Request.Email))
                .ReturnsAsync((User)null);

            Func<Task> action = async () => await _authService.LoginAsync(Request);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo("Invalid Credentials"));
        }

        [Test]
        public async Task Login_UserIsDeactivated_ThrowsUserDeactivatedException()
        {
            var Request = AuthTestUtil.CreateMockLoginRequestDto();

            var User = new User
            {
                Email = Request.Email,
                Password = "HashedPassword",
                IsDeleted = true
            };

            _userRepository
                .Setup(x => x.GetUserByEmailAsync(Request.Email))
                .ReturnsAsync(User);

            Func<Task> action = async () => await _authService.LoginAsync(Request);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo("User Deactivated"));
        }

        [Test]
        public async Task Login_InvalidPassword_ThrowsInvalidCredentialsException()
        {
            var Request = AuthTestUtil.CreateMockLoginRequestDto();

            var User = new User
            {
                Email = Request.Email,
                Password = Hasher.Hash("Password"),    
                IsDeleted = false
            };

            _userRepository
                .Setup(x => x.GetUserByEmailAsync(Request.Email))
                .ReturnsAsync(User);

            Func<Task> action = async () => await _authService.LoginAsync(Request);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo("Invalid Credentials"));
        }

        [Test]
        public async Task Login_ValidCredentials_ReturnsAccessAndRefreshTokens()
        {
            var Request = AuthTestUtil.CreateMockLoginRequestDto();

            ValidLoginHelper(Request);

            var response = await _authService.LoginAsync(Request);

            Assert.That(response.AccessToken, Is.EqualTo("AccessToken"));
            Assert.That(response.RefreshToken, Is.EqualTo("RefreshToken"));
        }

        [Test]
        public async Task Login_ValidCredentials_VerifiesPassword()
        {
            var Request = AuthTestUtil.CreateMockLoginRequestDto();

            var User = ValidLoginHelper(Request);

            await _authService.LoginAsync(Request);

            Hasher.Verify(Request.Password, User.Password);
        }

        [Test]
        public async Task Login_ValidCredentials_GeneratesAccessToken()
        {
            
            var Request = AuthTestUtil.CreateMockLoginRequestDto();

            ValidLoginHelper(Request);

            _authService.LoginAsync(Request);

            _jWTService.Verify(
                x => x.GetAccessToken(It.IsAny<User>()),
                Times.Once);
        }
        [Test]
        public async Task Login_ValidCredentials_GeneratesRefreshToken()
        {
            
            var Request = AuthTestUtil.CreateMockLoginRequestDto();

            ValidLoginHelper(Request);

            _authService.LoginAsync(Request);

            _jWTService.Verify(
                x => x.GenerateRefreshToken(),
                Times.Once);
        }
        [Test]
        public async Task Login_ValidCredentials_AddsRefreshToken()
        {
            
            var Request = AuthTestUtil.CreateMockLoginRequestDto();

            ValidLoginHelper(Request);

            _authService.LoginAsync(Request);

            _refreshTokenRepository.Verify(
                x => x.AddRefreshToken(It.IsAny<RefreshToken>()),
                Times.Once);
        }
        [Test]
        public async Task Login_ValidCredentials_SavesRefreshToken()
        {
            
            var Request = AuthTestUtil.CreateMockLoginRequestDto();

            ValidLoginHelper(Request);

            _authService.LoginAsync(Request);

            _appDbContext.Verify(
                x => x.SaveChanges(),
                Times.Once);
        }

        private User ValidLoginHelper(LoginRequestDto RequestDto)
        {
            var User = new User
            {
                UserId = Guid.NewGuid(),
                Email = RequestDto.Email,
                Password = Hasher.Hash(RequestDto.Password),
                IsDeleted = false
            };

            _userRepository
                .Setup(x => x.GetUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(User);

            _jWTService
                .Setup(x => x.GetAccessToken(It.IsAny<User>()))
                .Returns("AccessToken");

            _jWTService
                .Setup(x => x.GenerateRefreshToken())
                .Returns("RefreshToken");

            return User;
        }
    }
}
