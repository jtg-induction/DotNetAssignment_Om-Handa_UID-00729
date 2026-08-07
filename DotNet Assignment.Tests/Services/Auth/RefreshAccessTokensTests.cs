using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Repository.RefreshTokens;
using DotNet_Assignment.Repository.Users;
using DotNet_Assignment.Services.Auth;
using DotNet_Assignment.Services.JWT;
using DotNet_Assignment.Services.PasswordService;
using Moq;
using NUnit.Framework;
using System;

namespace DotNet_Assignment.Tests.Services.Auth
{
    [TestFixture]
    public class RefreshAccessTokensTests
    {

        private Mock<IUserRepository> _userRepository;
        private Mock<IJWTService> _jWTService;
        private Mock<IPasswordService> _passwordService;
        private Mock<IRefreshTokenRepository> _RefreshTokenRepository;
        private AuthService _authService;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _jWTService = new Mock<IJWTService>();
            _passwordService = new Mock<IPasswordService>();
            _RefreshTokenRepository = new Mock<IRefreshTokenRepository>();

            _authService = new AuthService(
                _userRepository.Object,
                _passwordService.Object,
                _jWTService.Object,
                _RefreshTokenRepository.Object
            );
        }

        [Test]
        public void RefreshAccessToken_InvalidRefreshToken_ThrowsException()
        {
            var RefreshToken = "RefreshToken";

            _RefreshTokenRepository
                .Setup(x => x.GetRefreshToken(RefreshToken))
                .Returns((RefreshToken)null);

            Action action = () => _authService.RefreshAccessToken(RefreshToken);

            var ex = Assert.Throws<Exception>(action);

            Assert.That(ex.Message, Is.EqualTo("Invalid Refresh Token"));
        }

        [Test]
        public void RefreshAccessToken_ExpiredRefreshToken_ThrowsException()
        {
            var RefreshToken = "RefreshToken";

            var token = new RefreshToken
            {
                Token = RefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(-5),
                User = new User()
            };

            _RefreshTokenRepository
                .Setup(x => x.GetRefreshToken(RefreshToken))
                .Returns(token);

            Action action = () => _authService.RefreshAccessToken(RefreshToken);

            var ex = Assert.Throws<Exception>(action);

            Assert.That(ex.Message, Is.EqualTo("Refresh Token Expired"));
        }

        [Test]
        public void RefreshAccessToken_DeactivatedUser_ThrowsException()
        {
            var RefreshToken = "RefreshToken";

            var token = new RefreshToken
            {
                Token = RefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                User = new User
                {
                    IsDeleted = true
                }
            };

            _RefreshTokenRepository
                .Setup(x => x.GetRefreshToken(RefreshToken))
                .Returns(token);

            Action action = () => _authService.RefreshAccessToken(RefreshToken);

            var ex = Assert.Throws<Exception>(action);

            Assert.That(ex.Message, Is.EqualTo("User Account is Deactivated"));
        }

        [Test]
        public void RefreshAccessToken_ValidToken_ReturnsNewAccessToken()
        {
            var RefreshToken = "RefreshToken";

            var user = new User
            {
                UserId = Guid.NewGuid(),
                IsDeleted = false
            };

            var token = new RefreshToken
            {
                Token = RefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                User = user
            };

            _RefreshTokenRepository
                .Setup(x => x.GetRefreshToken(RefreshToken))
                .Returns(token);

            _jWTService
                .Setup(x => x.GetAccessToken(user))
                .Returns("NewAccessToken");

            var response = _authService.RefreshAccessToken(RefreshToken);

            Assert.That(response.AccessToken, Is.EqualTo("NewAccessToken"));
            Assert.That(response.RefreshToken, Is.EqualTo(RefreshToken));
        }

        [Test]
        public void RefreshAccessToken_GeneratesNewAccessToken()
        {
            var RefreshToken = "RefreshToken";

            var user = new User
            {
                UserId = Guid.NewGuid(),
                IsDeleted = false
            };

            var token = new RefreshToken
            {
                Token = RefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                User = user
            };

            _RefreshTokenRepository
                .Setup(x => x.GetRefreshToken(RefreshToken))
                .Returns(token);

            _jWTService
                .Setup(x => x.GetAccessToken(user))
                .Returns("NewAccessToken");

            _authService.RefreshAccessToken(RefreshToken);

            _jWTService.Verify(
                x => x.GetAccessToken(user),
                Times.Once);
        }

    }
}
