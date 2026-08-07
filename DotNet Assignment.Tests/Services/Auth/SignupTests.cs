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
            var RequestDto = new SignupRequestDto
            {
                Email = "unittest@gmail.com"
            };

            _userRepository.
                Setup(x => x.FindUserByEmailAsync(RequestDto.Email)).
                ReturnsAsync(true);

            Func<Task> Action= async() => await _authService.RegisterAsync(RequestDto);

            var Exception= Assert.CatchAsync<Exception>(Action);

            Assert.That(Exception.Message, Is.EqualTo("Email Already Exists"));
        }

        [Test]
        public async Task Register_ValidUser_HashesPassword()
        {
            var RequestDto = AuthTestUtil.CreateMockSignupRequestDto();

            _userRepository.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>())).
                ReturnsAsync(false);

            _jWTService.
                Setup(x => x.GetAccessToken(It.IsAny<User>())).
                Returns("AccessToken");

            _jWTService.
                Setup(x => x.GenerateRefreshToken()).
                Returns("RefreshToken");

            await _authService.RegisterAsync(RequestDto);
        }

        [Test]
        public async Task Register_AddsUser()
        {
            var RequestDto = AuthTestUtil.CreateMockSignupRequestDto();

            _userRepository.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>())).
                ReturnsAsync(false);

            _jWTService
                .Setup(x => x.GetAccessToken(It.IsAny<User>()))
                .Returns("AccessToken");

            _jWTService
                .Setup(x => x.GenerateRefreshToken())
                .Returns("RefreshToken");

            await _authService.RegisterAsync(RequestDto);

            _userRepository.Verify(
                x => x.AddUser(It.IsAny<User>()),
                Times.Once);
        }

        [Test]
        public async Task Register_ValidUser_GeneratesAccessToken()
        {
            var RequestDto = AuthTestUtil.CreateMockSignupRequestDto();

            _userRepository.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>())).
                ReturnsAsync(false);

            _jWTService
                .Setup(x => x.GetAccessToken(It.IsAny<User>()))
                .Returns("AccessToken");

            _jWTService
                .Setup(x => x.GenerateRefreshToken())
                .Returns("RefreshToken");

            await _authService.RegisterAsync(RequestDto);

            _jWTService.Verify(x => x.GetAccessToken(It.IsAny<User>()),Times.Once);
        }

        [Test]
        public async Task Register_ValidUser_GeneratesRefreshToken()
        {
            var RequestDto = AuthTestUtil.CreateMockSignupRequestDto();

            _userRepository.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>())).
                ReturnsAsync(false);

            _jWTService
                .Setup(x => x.GetAccessToken(It.IsAny<User>()))
                .Returns("AccessToken");

            _jWTService
                .Setup(x => x.GenerateRefreshToken())
                .Returns("RefreshToken");

            await _authService.RegisterAsync(RequestDto);

            _jWTService.Verify(x => x.GenerateRefreshToken(),Times.Once);
        }

        [Test]
        public async Task Register_ValidUser_AddsRefreshToken()
        {
            var RequestDto = AuthTestUtil.CreateMockSignupRequestDto();

            _userRepository.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>())).
                ReturnsAsync(false);

            _jWTService
                .Setup(x => x.GetAccessToken(It.IsAny<User>()))
                .Returns("AccessToken");

            _jWTService
                .Setup(x => x.GenerateRefreshToken())
                .Returns("RefreshToken");

            await _authService.RegisterAsync(RequestDto);

            _refreshTokenRepository.Verify(x => x.AddRefreshToken(It.IsAny<RefreshToken>()),Times.Once);
        }

        [Test]
        public async Task Register_SavesChanges()
        {
            var RequestDto = AuthTestUtil.CreateMockSignupRequestDto();

            _userRepository.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>())).
                ReturnsAsync(false);

            _jWTService
                .Setup(x => x.GetAccessToken(It.IsAny<User>()))
                .Returns("AccessToken");

            _jWTService
                .Setup(x => x.GenerateRefreshToken())
                .Returns("RefreshToken");

            await _authService.RegisterAsync(RequestDto);

            _appDbContext.Verify(x => x.SaveChangesAsync(),Times.Once);
        }

        [Test]
        public async Task Register_ValidUser_ReturnTokens()
        {
            var RequestDto = AuthTestUtil.CreateMockSignupRequestDto();

            _userRepository.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>())).
                ReturnsAsync(false);

            _jWTService
                .Setup(x => x.GetAccessToken(It.IsAny<User>()))
                .Returns("AccessToken");

            _jWTService
                .Setup(x => x.GenerateRefreshToken())
                .Returns("RefreshToken");

            var Response = await _authService.RegisterAsync(RequestDto);

            Assert.That(Response.AccessToken, Is.EqualTo("AccessToken"));

            Assert.That(Response.RefreshToken, Is.EqualTo("RefreshToken"));

        }
    }
}
