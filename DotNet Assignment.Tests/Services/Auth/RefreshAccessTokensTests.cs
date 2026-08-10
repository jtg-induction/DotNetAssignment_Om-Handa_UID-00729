using DotNet_Assignment.Data;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Repository.RefreshTokens;
using DotNet_Assignment.Repository.Users;
using DotNet_Assignment.Services.Auth;
using DotNet_Assignment.Services.JWT;
using DotNet_Assignment.Utils;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace DotNet_Assignment.Tests.Services.Auth
{
    [TestFixture]
    public class RefreshAccessTokensTests
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
        public async Task RefreshAccessToken_InvalidRefreshToken_ThrowsException()
        {
            var refreshToken = "RefreshToken";
            var hashedToken = "HashedRefreshToken";

            _refreshTokenRepository
                .Setup(x => x.HashRefreshToken(refreshToken))
                .Returns(hashedToken);

            _refreshTokenRepository
                .Setup(x => x.GetRefreshTokenAsync(hashedToken))
                .ReturnsAsync((RefreshToken)null);

            Func<Task> action = async () => await _authService.RefreshAccessTokenAsync(refreshToken);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo("Refresh token invalid"));
        }

        [Test]
        public async Task RefreshAccessToken_ExpiredRefreshToken_ThrowsException()
        {
            var refreshToken = "RefreshToken";
            var hashedToken = "HashedRefreshToken";

            _refreshTokenRepository
                .Setup(x => x.HashRefreshToken(refreshToken))
                .Returns(hashedToken);

            var token = new RefreshToken
            {
                Token = hashedToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(-5),
                User = new User()
            };

            _refreshTokenRepository
                .Setup(x => x.GetRefreshTokenAsync(hashedToken))
                .ReturnsAsync(token);

            Func<Task> action = async () => await _authService.RefreshAccessTokenAsync(refreshToken);

            var Exception = Assert.CatchAsync<Exception>(action);

            Assert.That(Exception.Message, Is.EqualTo("Refresh token Expired"));
        }

        [Test]
        public async Task RefreshAccessToken_DeactivatedUser_ThrowsException()
        {
            var refreshToken = "RefreshToken";
            var hashedToken = "HashedRefreshToken";

            _refreshTokenRepository
                .Setup(x => x.HashRefreshToken(refreshToken))
                .Returns(hashedToken);

            var token = new RefreshToken
            {
                Token = hashedToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                User = new User
                {
                    IsDeleted = true
                }
            };

            _refreshTokenRepository
                .Setup(x => x.GetRefreshTokenAsync(hashedToken))
                .ReturnsAsync(token);

            Func<Task> action = async () => await _authService.RefreshAccessTokenAsync(refreshToken);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo("User is deactivated"));
        }

        [Test]
        public async Task RefreshAccessToken_ValidToken_ReturnsNewAccessToken()
        {
            var refreshToken = "RefreshToken";
            var hashedToken = "HashedRefreshToken";

            _refreshTokenRepository
                .Setup(x => x.HashRefreshToken(refreshToken))
                .Returns(hashedToken);

            var user = new User
            {
                UserId = Guid.NewGuid(),
                IsDeleted = false
            };

            var token = new RefreshToken
            {
                Token = hashedToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                User = user
            };

            _refreshTokenRepository
                .Setup(x => x.GetRefreshTokenAsync(hashedToken))
                .ReturnsAsync(token);

            _jWTService
                .Setup(x => x.GetAccessToken(user))
                .Returns("NewAccessToken");

            var response = await _authService.RefreshAccessTokenAsync(refreshToken);

            Assert.That(response.AccessToken, Is.EqualTo("NewAccessToken"));
            Assert.That(response.RefreshToken, Is.EqualTo(refreshToken));
        }

        [Test]
        public async Task RefreshAccessToken_GeneratesNewAccessToken()
        {
            var refreshToken = "RefreshToken";
            var hashedToken = "HashedRefreshToken";

            _refreshTokenRepository
                .Setup(x => x.HashRefreshToken(refreshToken))
                .Returns(hashedToken);

            var user = new User
            {
                UserId = Guid.NewGuid(),
                IsDeleted = false
            };

            var token = new RefreshToken
            {
                Token = hashedToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                User = user
            };

            _refreshTokenRepository
                .Setup(x => x.GetRefreshTokenAsync(hashedToken))
                .ReturnsAsync(token);

            _jWTService
                .Setup(x => x.GetAccessToken(user))
                .Returns("NewAccessToken");

            await _authService.RefreshAccessTokenAsync(refreshToken);

            _jWTService.Verify(
                x => x.GetAccessToken(user),
                Times.Once);
        }
    }
}
