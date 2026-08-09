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
            var request = AuthTestUtil.CreateMockLoginRequestDto();

            _userRepository
                .Setup(x => x.GetUserByEmailAsync(request.Email))
                .ReturnsAsync((User)null);

            Func<Task> action = async () => await _authService.LoginAsync(request);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo("Invalid Credentials"));
        }

        [Test]
        public async Task Login_UserIsDeactivated_ThrowsUserDeactivatedException()
        {
            var request = AuthTestUtil.CreateMockLoginRequestDto();

            var user = new User
            {
                Email = request.Email,
                Password = "HashedPassword",
                IsDeleted = true
            };

            _userRepository
                .Setup(x => x.GetUserByEmailAsync(request.Email))
                .ReturnsAsync(user);

            Func<Task> action = async () => await _authService.LoginAsync(request);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo("User Deactivated"));
        }

        [Test]
        public async Task Login_InvalidPassword_ThrowsInvalidCredentialsException()
        {
            var request = AuthTestUtil.CreateMockLoginRequestDto();

            var user = new User
            {
                Email = request.Email,
                Password = Hasher.Hash("Password"),    
                IsDeleted = false
            };

            _userRepository
                .Setup(x => x.GetUserByEmailAsync(request.Email))
                .ReturnsAsync(user);

            Func<Task> action = async () => await _authService.LoginAsync(request);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo("Invalid Credentials"));
        }

        [Test]
        public async Task Login_ValidCredentials_ReturnsAccessAndRefreshTokens()
        {
            var request = AuthTestUtil.CreateMockLoginRequestDto();

            ValidLoginHelper(request);

            var response = await _authService.LoginAsync(request);

            Assert.That(response.AccessToken, Is.EqualTo("AccessToken"));
            Assert.That(response.RefreshToken, Is.EqualTo("RefreshToken"));
        }

        [Test]
        public async Task Login_ValidCredentials_VerifiesPassword()
        {
            var request = AuthTestUtil.CreateMockLoginRequestDto();

            var User = ValidLoginHelper(request);

            await _authService.LoginAsync(request);

            Hasher.Verify(request.Password, User.Password);
        }

        [Test]
        public async Task Login_ValidCredentials_GeneratesAccessToken()
        {
            
            var request = AuthTestUtil.CreateMockLoginRequestDto();

            ValidLoginHelper(request);

            _authService.LoginAsync(request);

            _jWTService.Verify(
                x => x.GetAccessToken(It.IsAny<User>()),
                Times.Once);
        }
        [Test]
        public async Task Login_ValidCredentials_GeneratesRefreshToken()
        {
            
            var request = AuthTestUtil.CreateMockLoginRequestDto();

            ValidLoginHelper(request);

            _authService.LoginAsync(request);

            _jWTService.Verify(
                x => x.GenerateRefreshToken(),
                Times.Once);
        }
        [Test]
        public async Task Login_ValidCredentials_AddsRefreshToken()
        {
            
            var request = AuthTestUtil.CreateMockLoginRequestDto();

            ValidLoginHelper(request);

            _authService.LoginAsync(request);

            _refreshTokenRepository.Verify(
                x => x.AddRefreshToken(It.IsAny<RefreshToken>()),
                Times.Once);
        }
        [Test]
        public async Task Login_ValidCredentials_SavesRefreshToken()
        {
            
            var request = AuthTestUtil.CreateMockLoginRequestDto();

            ValidLoginHelper(request);

            _authService.LoginAsync(request);

            _appDbContext.Verify(
                x => x.SaveChanges(),
                Times.Once);
        }

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

            _jWTService
                .Setup(x => x.GetAccessToken(It.IsAny<User>()))
                .Returns("AccessToken");

            _jWTService
                .Setup(x => x.GenerateRefreshToken())
                .Returns("RefreshToken");

            return user;
        }
    }
}
