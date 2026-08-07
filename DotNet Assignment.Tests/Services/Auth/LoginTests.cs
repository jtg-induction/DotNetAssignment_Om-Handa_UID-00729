using DotNet_Assignment.Repository.RefreshTokens;
using DotNet_Assignment.Repository.Users;
using DotNet_Assignment.Services.Auth;
using DotNet_Assignment.Services.JWT;
using DotNet_Assignment.Services.PasswordService;
using DotNet_Assignment.Tests.Utils;
using DotNet_Assignment.Models.Entities;
using Moq;
using NUnit.Framework;
using System;
using DotNet_Assignment.Models.DTO;

namespace DotNet_Assignment.Tests.Services.Auth
{
    [TestFixture]
    public class LoginTests
    {

        private Mock<IUserRepository> _userRepository;
        private Mock<IJWTService> _jWTService;
        private Mock<IPasswordService> _passwordService;
        private Mock<IRefreshTokenRepository> _refreshTokenRepository;
        private AuthService _authService;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _jWTService = new Mock<IJWTService>();
            _passwordService = new Mock<IPasswordService>();
            _refreshTokenRepository = new Mock<IRefreshTokenRepository>();

            _authService = new AuthService(
                _userRepository.Object,
                _passwordService.Object,
                _jWTService.Object,
                _refreshTokenRepository.Object
            );
        }

        [Test]
        public void Login_UserDoesNotExist_ThrowsInvalidCredentialsException()
        {
            var Request = AuthTestUtil.CreateMockLoginRequestDto();

            _userRepository
                .Setup(x => x.GetUserByEmail(Request.Email))
                .Returns((User)null);

            Action action = () => _authService.Login(Request);

            var exception = Assert.Throws<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo("Invalid Credentials"));
        }

        [Test]
        public void Login_UserIsDeactivated_ThrowsUserDeactivatedException()
        {
            var Request = AuthTestUtil.CreateMockLoginRequestDto();

            var User = new User
            {
                Email = Request.Email,
                Password = "HashedPassword",
                IsDeleted = true
            };

            _userRepository
                .Setup(x => x.GetUserByEmail(Request.Email))
                .Returns(User);

            Action action = () => _authService.Login(Request);

            var exception = Assert.Throws<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo("User Deactivated"));
        }

        [Test]
        public void Login_InvalidPassword_ThrowsInvalidCredentialsException()
        {
            var Request = AuthTestUtil.CreateMockLoginRequestDto();

            var User = new User
            {
                Email = Request.Email,
                Password = "HashedPassword",
                IsDeleted = false
            };

            _userRepository
                .Setup(x => x.GetUserByEmail(Request.Email))
                .Returns(User);

            _passwordService
                .Setup(x => x.VerifyPassword(Request.Password, User.Password))
                .Returns(false);

            Action action = () => _authService.Login(Request);

            var exception = Assert.Throws<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo("Invalid Credentials"));
        }

        [Test]
        public void Login_ValidCredentials_ReturnsAccessAndRefreshTokens()
        {
            var Request = AuthTestUtil.CreateMockLoginRequestDto();

            ValidLoginHelper(Request);

            var response = _authService.Login(Request);

            Assert.That(response.AccessToken, Is.EqualTo("access-token"));
            Assert.That(response.RefreshToken, Is.EqualTo("refresh-token"));
        }

        [Test]
        public void Login_ValidCredentials_VerifiesPassword()
        {
            var Request = AuthTestUtil.CreateMockLoginRequestDto();

            var User = ValidLoginHelper(Request);

            _authService.Login(Request);

            _passwordService.Verify(
                x => x.VerifyPassword(Request.Password, User.Password),
                Times.Once);
        }

        [Test]
        public void Login_ValidCredentials_GeneratesAccessToken()
        {
            
            var Request = AuthTestUtil.CreateMockLoginRequestDto();

            ValidLoginHelper(Request);

            _authService.Login(Request);

            _jWTService.Verify(
                x => x.GetAccessToken(It.IsAny<User>()),
                Times.Once);
        }
        [Test]
        public void Login_ValidCredentials_GeneratesRefreshToken()
        {
            
            var Request = AuthTestUtil.CreateMockLoginRequestDto();

            ValidLoginHelper(Request);

            _authService.Login(Request);

            _jWTService.Verify(
                x => x.GetRefreshToken(),
                Times.Once);
        }
        [Test]
        public void Login_ValidCredentials_AddsRefreshToken()
        {
            
            var Request = AuthTestUtil.CreateMockLoginRequestDto();

            ValidLoginHelper(Request);

            _authService.Login(Request);

            _refreshTokenRepository.Verify(
                x => x.AddRefreshToken(It.IsAny<RefreshToken>()),
                Times.Once);
        }
        [Test]
        public void Login_ValidCredentials_SavesRefreshToken()
        {
            
            var Request = AuthTestUtil.CreateMockLoginRequestDto();

            ValidLoginHelper(Request);

            _authService.Login(Request);

            _refreshTokenRepository.Verify(
                x => x.Save(),
                Times.Once);
        }

        private User ValidLoginHelper(LoginRequestDto RequestDto)
        {
            var User = new User
            {
                UserId = Guid.NewGuid(),
                Email = RequestDto.Email,
                Password = "HashedPassword",
                IsDeleted = false
            };

            _userRepository
                .Setup(x => x.GetUserByEmail(It.IsAny<string>()))
                .Returns(User);

            _passwordService
                .Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            _jWTService
                .Setup(x => x.GetAccessToken(It.IsAny<User>()))
                .Returns("access-token");

            _jWTService
                .Setup(x => x.GetRefreshToken())
                .Returns("refresh-token");

            return User;
        }
    }
}
