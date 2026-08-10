using DotNet_Assignment.Data;
using DotNet_Assignment.Models.DTO;
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
    public class SignupTests
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
        public async Task Register_EmailAlreadyExists_ThrowsException()
        {
            var requestDto = new SignupRequestDto
            {
                Email = "unittest@gmail.com"
            };

            _userRepository.
                Setup(x => x.FindUserByEmailAsync(requestDto.Email)).
                ReturnsAsync(true);

            Func<Task> Action= async() => await _authService.RegisterAsync(requestDto);

            var exception= Assert.CatchAsync<Exception>(Action);

            Assert.That(exception.Message, Is.EqualTo("Email already exists"));
        }

        [Test]
        public async Task Register_ValidUser_HashesPassword()
        {
            var requestDto = AuthTestUtil.CreateMockSignupRequestDto();

            _userRepository.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>())).
                ReturnsAsync(false);

            _jWTService.
                Setup(x => x.GetAccessToken(It.IsAny<User>())).
                Returns("AccessToken");

            _jWTService.
                Setup(x => x.GenerateRefreshToken()).
                Returns("RefreshToken");

            await _authService.RegisterAsync(requestDto);
        }

        [Test]
        public async Task Register_AddsUser()
        {
            var requestDto = AuthTestUtil.CreateMockSignupRequestDto();

            _userRepository.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>())).
                ReturnsAsync(false);

            _jWTService
                .Setup(x => x.GetAccessToken(It.IsAny<User>()))
                .Returns("AccessToken");

            _jWTService
                .Setup(x => x.GenerateRefreshToken())
                .Returns("RefreshToken");

            await _authService.RegisterAsync(requestDto);

            _userRepository.Verify(
                x => x.AddUser(It.IsAny<User>()),
                Times.Once);
        }

        [Test]
        public async Task Register_ValidUser_GeneratesAccessToken()
        {
            var requestDto = AuthTestUtil.CreateMockSignupRequestDto();

            _userRepository.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>())).
                ReturnsAsync(false);

            _jWTService
                .Setup(x => x.GetAccessToken(It.IsAny<User>()))
                .Returns("AccessToken");

            _jWTService
                .Setup(x => x.GenerateRefreshToken())
                .Returns("RefreshToken");

            await _authService.RegisterAsync(requestDto);

            _jWTService.Verify(x => x.GetAccessToken(It.IsAny<User>()),Times.Once);
        }

        [Test]
        public async Task Register_ValidUser_GeneratesRefreshToken()
        {
            var requestDto = AuthTestUtil.CreateMockSignupRequestDto();

            _userRepository.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>())).
                ReturnsAsync(false);

            _jWTService
                .Setup(x => x.GetAccessToken(It.IsAny<User>()))
                .Returns("AccessToken");

            _jWTService
                .Setup(x => x.GenerateRefreshToken())
                .Returns("RefreshToken");

            await _authService.RegisterAsync(requestDto);

            _jWTService.Verify(x => x.GenerateRefreshToken(),Times.Once);
        }

        [Test]
        public async Task Register_ValidUser_AddsRefreshToken()
        {
            var requestDto = AuthTestUtil.CreateMockSignupRequestDto();

            _userRepository.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>())).
                ReturnsAsync(false);

            _jWTService
                .Setup(x => x.GetAccessToken(It.IsAny<User>()))
                .Returns("AccessToken");

            _jWTService
                .Setup(x => x.GenerateRefreshToken())
                .Returns("RefreshToken");

            await _authService.RegisterAsync(requestDto);

            _refreshTokenRepository.Verify(x => x.AddRefreshToken(It.IsAny<RefreshToken>()),Times.Once);
        }

        [Test]
        public async Task Register_SavesChanges()
        {
            var requestDto = AuthTestUtil.CreateMockSignupRequestDto();

            _userRepository.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>())).
                ReturnsAsync(false);

            _jWTService
                .Setup(x => x.GetAccessToken(It.IsAny<User>()))
                .Returns("AccessToken");

            _jWTService
                .Setup(x => x.GenerateRefreshToken())
                .Returns("RefreshToken");

            await _authService.RegisterAsync(requestDto);

            _appDbContext.Verify(x => x.SaveChangesAsync(),Times.Once);
        }

        [Test]
        public async Task Register_ValidUser_ReturnTokens()
        {
            var requestDto = AuthTestUtil.CreateMockSignupRequestDto();

            _userRepository.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>())).
                ReturnsAsync(false);

            _jWTService
                .Setup(x => x.GetAccessToken(It.IsAny<User>()))
                .Returns("AccessToken");

            _jWTService
                .Setup(x => x.GenerateRefreshToken())
                .Returns("RefreshToken");

            var response = await _authService.RegisterAsync(requestDto);

            Assert.That(response.AccessToken, Is.EqualTo("AccessToken"));

            Assert.That(response.RefreshToken, Is.EqualTo("RefreshToken"));

        }
    }
}
