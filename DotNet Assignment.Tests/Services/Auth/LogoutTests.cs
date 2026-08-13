using DotNet_Assignment.Data;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Repository.RefreshTokens;
using DotNet_Assignment.Repository.Users;
using DotNet_Assignment.Services.Auth;
using DotNet_Assignment.Services.JWT;
using DotNet_Assignment.Tests.Utils;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace DotNet_Assignment.Tests.Services.Auth
{
    [TestFixture]
    public class LogoutTests
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
        /// Logout Function - Refresh Token not Found - Returns Function
        /// </summary>
        [Test]
        public void Logout_RefreshTokenNotFound_Returns()
        {

            var request = AuthTestUtil.CreateMockLogoutRequestDto();

            _refreshTokenRepository
                .Setup(x => x.GetRefreshTokenAsync(request.RefreshToken))
                .ReturnsAsync((RefreshToken)null);

            _authService.LogoutAsync(request);

            _refreshTokenRepository.Verify(
                x => x.DeleteRefreshToken(It.IsAny<RefreshToken>()),
                Times.Never);

            _appDbContext.Verify(x => x.SaveChanges(),Times.Never);
        }

        /// <summary>
        /// Logout Function - Refresh Token Found - Deletes Token
        /// </summary>
        [Test]
        public async Task Logout_ValidRefreshToken_DeletesRefreshToken()
        {

            var request = AuthTestUtil.CreateMockLogoutRequestDto();

            var refreshToken = new RefreshToken
            {
                RefreshTokenId = Guid.NewGuid(),
                Token = request.RefreshToken,
                UserId = Guid.NewGuid()
            };

            _refreshTokenRepository
                .Setup(x => x.HashRefreshToken(request.RefreshToken))
                .Returns(refreshToken.Token);

            _refreshTokenRepository
                .Setup(x => x.GetRefreshTokenAsync(request.RefreshToken))
                .ReturnsAsync(refreshToken);

            await _authService.LogoutAsync(request);

            _refreshTokenRepository.Verify(x => x.DeleteRefreshToken(refreshToken),Times.Once);

            _appDbContext.Verify(x => x.SaveChanges(), Times.Once);
        }

        /// <summary>
        /// Logout Function - Refresh Token Found - Saves Changes
        /// </summary>
        [Test]
        public async Task Logout_ValidRefreshToken_SavesChanges()
        {

            var request = AuthTestUtil.CreateMockLogoutRequestDto();

            var refreshToken = new RefreshToken
            {
                RefreshTokenId = Guid.NewGuid(),
                Token = request.RefreshToken,
                UserId = Guid.NewGuid()
            };

            _refreshTokenRepository
                .Setup(x => x.HashRefreshToken(request.RefreshToken))
                .Returns(refreshToken.Token);

            _refreshTokenRepository
                .Setup(x => x.GetRefreshTokenAsync(request.RefreshToken))
                .ReturnsAsync(refreshToken);

            await _authService.LogoutAsync(request);

            _appDbContext.Verify(
                x => x.SaveChanges(),
                Times.Once);
        }

    }
}
