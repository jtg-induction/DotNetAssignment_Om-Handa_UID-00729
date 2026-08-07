using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Repository.RefreshTokens;
using DotNet_Assignment.Repository.Users;
using DotNet_Assignment.Services.Auth;
using DotNet_Assignment.Services.JWT;
using DotNet_Assignment.Services.PasswordService;
using DotNet_Assignment.Tests.Utils;
using Moq;
using NUnit.Framework;
using System;

namespace DotNet_Assignment.Tests.Services.Auth
{
    [TestFixture]
    public class LogoutTests
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
        public void Logout_RefreshTokenNotFound_Returns()
        {

            var request = AuthTestUtil.CreateMockLogoutRequestDto();

            _refreshTokenRepository
                .Setup(x => x.GetRefreshToken(request.RefreshToken))
                .Returns((RefreshToken)null);

            _authService.Logout(request);

            _refreshTokenRepository.Verify(
                x => x.DeleteRefreshToken(It.IsAny<RefreshToken>()),
                Times.Never);

            _refreshTokenRepository.Verify(
                x => x.Save(),
                Times.Never);
        }

        [Test]
        public void Logout_ValidRefreshToken_DeletesRefreshToken()
        {

            var request = AuthTestUtil.CreateMockLogoutRequestDto();

            var refreshToken = new RefreshToken
            {
                RefreshTokenId = Guid.NewGuid(),
                Token = request.RefreshToken,
                UserId = Guid.NewGuid()
            };

            _refreshTokenRepository
                .Setup(x => x.GetRefreshToken(request.RefreshToken))
                .Returns(refreshToken);

            _authService.Logout(request);

            _refreshTokenRepository.Verify(
                x => x.DeleteRefreshToken(refreshToken),
                Times.Once);
        }

        [Test]
        public void Logout_ValidRefreshToken_SavesChanges()
        {

            var request = AuthTestUtil.CreateMockLogoutRequestDto();

            var refreshToken = new RefreshToken
            {
                RefreshTokenId = Guid.NewGuid(),
                Token = request.RefreshToken,
                UserId = Guid.NewGuid()
            };

            _refreshTokenRepository
                .Setup(x => x.GetRefreshToken(request.RefreshToken))
                .Returns(refreshToken);

            _authService.Logout(request);

            _refreshTokenRepository.Verify(
                x => x.Save(),
                Times.Once);
        }

    }
}
