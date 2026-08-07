using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Repository.RefreshTokens;
using DotNet_Assignment.Repository.Users;
using DotNet_Assignment.Services.JWT;
using DotNet_Assignment.Services.PasswordService;
using Moq;
using NUnit.Framework;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Services.Auth;
using System;
using DotNet_Assignment.Tests.Utils;

namespace DotNet_Assignment.Tests.Services.Auth
{
    [TestFixture]
    public class SignupTests
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
        public void Register_EmailAlreadyExists_ThrowsException()
        {
            var RequestDto = new SignupRequestDto
            {
                Email = "unittest@gmail.com"
            };

            _userRepository.
                Setup(x => x.GetUserByEmail(RequestDto.Email)).
                Returns(new User());

            Action Action= () => _authService.Register(RequestDto);

            var Exception= Assert.Throws<Exception>(Action);

            Assert.That(Exception.Message, Is.EqualTo("User Already Exists"));
        }

        [Test]
        public void Register_ValidUser_HashesPassword()
        {
            var RequestDto = AuthTestUtil.CreateMockSignupRequestDto();

            _userRepository.Setup(x => x.GetUserByEmail(It.IsAny<string>())).
                Returns((User)null);

            _passwordService.Setup(x => x.HashPassword(It.IsAny<string>())).
                Returns("HashedPassword");

            _jWTService.
                Setup(x => x.GetAccessToken(It.IsAny<User>())).
                Returns("AccessToken");

            _jWTService.
                Setup(x => x.GetRefreshToken()).
                Returns("RefreshToken");

            _authService.
                Register(RequestDto);

            _passwordService.Verify(x => x.HashPassword(RequestDto.Password),Times.Once);
        }

        [Test]
        public void Register_AddsUser()
        {
            var RequestDto = AuthTestUtil.CreateMockSignupRequestDto();

            _userRepository
                .Setup(x => x.GetUserByEmail(It.IsAny<string>()))
                .Returns((User)null);

            _passwordService
                .Setup(x => x.HashPassword(It.IsAny<string>()))
                .Returns("HashPassword");

            _jWTService
                .Setup(x => x.GetAccessToken(It.IsAny<User>()))
                .Returns("AccessToken");

            _jWTService
                .Setup(x => x.GetRefreshToken())
                .Returns("RefreshToken");

            _authService.Register(RequestDto);

            _userRepository.Verify(
                x => x.AddUser(It.IsAny<User>()),
                Times.Once);
        }

        [Test]
        public void Register_SavesUser()
        {
            var RequestDto = AuthTestUtil.CreateMockSignupRequestDto();

            _userRepository
                .Setup(x => x.GetUserByEmail(It.IsAny<string>()))
                .Returns((User)null);

            _passwordService
                .Setup(x => x.HashPassword(It.IsAny<string>()))
                .Returns("HashedPassword");

            _jWTService
                .Setup(x => x.GetAccessToken(It.IsAny<User>()))
                .Returns("AccessToken");

            _jWTService
                .Setup(x => x.GetRefreshToken())
                .Returns("RefreshToken");

            _authService.Register(RequestDto);

            _userRepository.Verify(
                x => x.Save(),
                Times.Once);
        }

        [Test]
        public void Register_ValidUser_GeneratesAccessToken()
        {
            var RequestDto = AuthTestUtil.CreateMockSignupRequestDto();

            _userRepository
                .Setup(x => x.GetUserByEmail(It.IsAny<string>()))
                .Returns((User)null);

            _passwordService
                .Setup(x => x.HashPassword(It.IsAny<string>()))
                .Returns("HashedPassword");

            _jWTService
                .Setup(x => x.GetAccessToken(It.IsAny<User>()))
                .Returns("AccessToken");

            _jWTService
                .Setup(x => x.GetRefreshToken())
                .Returns("RefreshToken");

            _authService.Register(RequestDto);

            _jWTService.Verify(x => x.GetAccessToken(It.IsAny<User>()),Times.Once);
        }

        [Test]
        public void Register_ValidUser_GeneratesRefreshToken()
        {
            var RequestDto = AuthTestUtil.CreateMockSignupRequestDto();

            _userRepository
                .Setup(x => x.GetUserByEmail(It.IsAny<string>()))
                .Returns((User)null);

            _passwordService
                .Setup(x => x.HashPassword(It.IsAny<string>()))
                .Returns("HashedPassword");

            _jWTService
                .Setup(x => x.GetAccessToken(It.IsAny<User>()))
                .Returns("AccessToken");

            _jWTService
                .Setup(x => x.GetRefreshToken())
                .Returns("RefreshToken");

            _authService.Register(RequestDto);

            _jWTService.Verify(x => x.GetRefreshToken(),Times.Once);
        }

        [Test]
        public void Register_ValidUser_AddsRefreshToken()
        {
            var RequestDto = AuthTestUtil.CreateMockSignupRequestDto();

            _userRepository
                .Setup(x => x.GetUserByEmail(It.IsAny<string>()))
                .Returns((User)null);

            _passwordService
                .Setup(x => x.HashPassword(It.IsAny<string>()))
                .Returns("HashedPassword");

            _jWTService
                .Setup(x => x.GetAccessToken(It.IsAny<User>()))
                .Returns("AccessToken");

            _jWTService
                .Setup(x => x.GetRefreshToken())
                .Returns("RefreshToken");

            _authService.Register(RequestDto);

            _refreshTokenRepository.Verify(x => x.AddRefreshToken(It.IsAny<RefreshToken>()),Times.Once);
        }

        [Test]
        public void Register_SavesRefreshToken()
        {
            var RequestDto = AuthTestUtil.CreateMockSignupRequestDto();

            _userRepository
                .Setup(x => x.GetUserByEmail(It.IsAny<string>()))
                .Returns((User)null);

            _passwordService
                .Setup(x => x.HashPassword(It.IsAny<string>()))
                .Returns("HashedPassword");

            _jWTService
                .Setup(x => x.GetAccessToken(It.IsAny<User>()))
                .Returns("AccessToken");

            _jWTService
                .Setup(x => x.GetRefreshToken())
                .Returns("RefreshToken");

            _authService.Register(RequestDto);

            _refreshTokenRepository.Verify(x => x.Save(),Times.Once);
        }

        [Test]
        public void Register_ValidUser_ReturnTokens()
        {
            var RequestDto = AuthTestUtil.CreateMockSignupRequestDto();

            _userRepository
                .Setup(x => x.GetUserByEmail(RequestDto.Email))
                .Returns((User)null);

            _passwordService
                .Setup(x => x.HashPassword(RequestDto.Password))
                .Returns("HashedPassword");

            _jWTService
                .Setup(x => x.GetAccessToken(It.IsAny<User>()))
                .Returns("AccessToken");

            _jWTService
                .Setup(x => x.GetRefreshToken())
                .Returns("RefreshToken");

            var Response = _authService.Register(RequestDto);

            Assert.That(Response.AccessToken, Is.EqualTo("AccessToken"));

            Assert.That(Response.RefreshToken, Is.EqualTo("RefreshToken"));

        }
    }
}
