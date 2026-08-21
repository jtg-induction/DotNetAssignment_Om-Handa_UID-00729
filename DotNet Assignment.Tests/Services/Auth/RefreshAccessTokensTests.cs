using DotNet_Assignment.Constants;
using DotNet_Assignment.Data;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Repository.RefreshTokens;
using DotNet_Assignment.Repository.Users;
using DotNet_Assignment.Services.Auth;
using DotNet_Assignment.Services.JWT;
using DotNet_Assignment.Tests.Constants;
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

        /// <summary>
        /// RefreshAccessToken - Invalid Refresh Token - throws exception
        /// </summary>
        [Test]
        public async Task RefreshAccessToken_InvalidRefreshToken_ThrowsException()
        {
            var refreshToken = MockConstants.MockRefreshToken;
            var hashedToken = MockConstants.MockHashedRefreshToken;

            _refreshTokenRepository
                .Setup(x => x.HashRefreshToken(refreshToken))
                .Returns(hashedToken);

            _refreshTokenRepository
                .Setup(x => x.GetRefreshTokenAsync(hashedToken))
                .ReturnsAsync((RefreshToken)null);

            Func<Task> Action = async () => await _authService.RefreshAccessTokenAsync(refreshToken);

            var exception = Assert.CatchAsync<Exception>(Action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.RefreshTokenInvalid));
        }

        /// <summary>
        /// RefreshAccessToken - Expired Refresh Token - throws exception
        /// </summary>
        [Test]
        public async Task RefreshAccessToken_ExpiredRefreshToken_ThrowsException()
        {
            var refreshToken = MockConstants.MockRefreshToken;
            var hashedToken = MockConstants.MockHashedRefreshToken;

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

            Func<Task> Action = async () => await _authService.RefreshAccessTokenAsync(refreshToken);

            var exception = Assert.CatchAsync(Action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.RefreshTokenExpired));
        }

        /// <summary>
        /// RefreshAccessToken - User Deactivated - throws exception
        /// </summary>
        [Test]
        public async Task RefreshAccessToken_DeactivatedUser_ThrowsException()
        {
            var refreshToken = MockConstants.MockRefreshToken;
            var hashedToken = MockConstants.MockHashedRefreshToken;

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

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.UserDeactivated));
        }


        /// <summary>
        /// RefreshAccessToken - Valid Refresh Token - returns new access token
        /// </summary>
        [Test]
        public async Task RefreshAccessToken_ValidToken_ReturnsNewAccessToken()
        {
            var refreshToken = MockConstants.MockRefreshToken;
            var hashedToken = MockConstants.MockHashedRefreshToken;

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
                .Returns(MockConstants.MockNewAccessToken);

            Func<Task> Action = async () => await _authService.RefreshAccessTokenAsync(refreshToken);

            Assert.DoesNotThrowAsync(Action);

            var response = await _authService.RefreshAccessTokenAsync(refreshToken);

            Assert.That(response.AccessToken, Is.EqualTo(MockConstants.MockNewAccessToken));
            Assert.That(response.RefreshToken, Is.EqualTo(refreshToken));
        }


        /// <summary>
        /// RefreshAccessToken - Valid Refresh Token - Generates New access token
        /// </summary>
        [Test]
        public async Task RefreshAccessToken_GeneratesNewAccessToken()
        {
            var refreshToken = MockConstants.MockRefreshToken;
            var hashedToken = MockConstants.MockHashedRefreshToken;

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
                .Returns(MockConstants.MockNewAccessToken);

            Func<Task> Action = async () => await _authService.RefreshAccessTokenAsync(refreshToken);

            Assert.DoesNotThrowAsync(Action);

            _jWTService.Verify(
                x => x.GetAccessToken(user),
                Times.Once);
        }
    }
}
