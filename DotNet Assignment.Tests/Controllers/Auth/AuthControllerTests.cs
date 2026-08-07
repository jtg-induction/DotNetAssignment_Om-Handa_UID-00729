using DotNet_Assignment.Controllers;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Services.Auth;
using DotNet_Assignment.Tests.Utils;
using Moq;
using NUnit.Framework;
using System;

namespace DotNet_Assignment.Tests.Controllers.Auth
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<IAuthService> _authService;
        private AuthController _authController;

        [SetUp]
        public void Setup()
        {
            _authService = new Mock<IAuthService>();

            _authController = new AuthController(_authService.Object);
        }

        [Test]
        public void SignUp_ValidRequest_ReturnsSuccess()
        {
            var RequestDto = AuthTestUtil.CreateMockSignupRequestDto();

            var Response = new JWTResponseDto
            {
                AccessToken = "AccessToken",
                RefreshToken = "RefreshToken"
            };

            _authService.Setup(x => x.Register(RequestDto))
                        .Returns(Response);

            var Result = _authController.SignUp(RequestDto);

            Assert.That(Result.IsSuccess, Is.True);
            Assert.That("User Signed In", Is.EqualTo(Result.Message));
            Assert.That(Response, Is.EqualTo(Result.Data));
        }

        [Test]
        public void SignUp_ServiceThrows_ReturnsFailure()
        {
            var RequestDto = AuthTestUtil.CreateMockSignupRequestDto();

            _authService.Setup(x => x.Register(RequestDto))
                        .Throws(new Exception("User Already Exists"));

            var Result = _authController.SignUp(RequestDto);

            Assert.That(Result.IsSuccess, Is.False);
            Assert.That("User Already Exists", Is.EqualTo(Result.Message));
        }

        [Test]
        public void SignUp_InvalidModelState_ThrowsException()
        {
            var RequestDto = AuthTestUtil.CreateMockSignupRequestDto();

            _authController.ModelState.AddModelError("Email", "Required");

            Action action = () => _authController.SignUp(RequestDto);

            var Exception = Assert.Throws<Exception>(action);

            Assert.That(Exception.Message, Is.EqualTo("Wrong Details"));
        }

        [Test]
        public void Login_ValidRequest_ReturnsSuccess()
        {
            var RequestDto = AuthTestUtil.CreateMockLoginRequestDto();

            var Response = new JWTResponseDto
            {
                AccessToken = "AccessToken",
                RefreshToken = "RefreshToken"
            };

            _authService.Setup(x => x.Login(RequestDto))
                        .Returns(Response);

            var Result = _authController.Login(RequestDto);

            Assert.That(Result.IsSuccess, Is.True);
            Assert.That("User Logged In", Is.EqualTo(Result.Message));
            Assert.That(Response, Is.EqualTo(Result.Data));
        }

        [Test]
        public void Login_ServiceThrows_ReturnsFailure()
        {
            var RequestDto = AuthTestUtil.CreateMockLoginRequestDto();

            _authService.Setup(x => x.Login(RequestDto))
                        .Throws(new Exception("Invalid Credentials"));

            var Result = _authController.Login(RequestDto);

            Assert.That(Result.IsSuccess, Is.False);
            Assert.That("Invalid Credentials", Is.EqualTo(Result.Message));
        }

        [Test]
        public void Login_InvalidModelState_ThrowsException()
        {
            var RequestDto = AuthTestUtil.CreateMockLoginRequestDto();

            _authController.ModelState.AddModelError("Email", "Required");

            Action action = () => _authController.Login(RequestDto);

            var Exception = Assert.Throws<Exception>(action);

            Assert.That(Exception.Message, Is.EqualTo("Wrong Details"));
        }

        [Test]
        public void Logout_ValidRequest_ReturnsSuccess()
        {
            var RequestDto = new LogoutRequestDto
            {
                RefreshToken = "RefreshToken"
            };

            var Result = _authController.Logout(RequestDto);

            Assert.That(Result.IsSuccess, Is.True);
            Assert.That("User Logged Out", Is.EqualTo(Result.Message));

            _authService.Verify(x => x.Logout(RequestDto), Times.Once);
        }

        [Test]
        public void Logout_ServiceThrows_ReturnsFailure()
        {
            var RequestDto = new LogoutRequestDto
            {
                RefreshToken = "RefreshToken"
            };

            _authService.Setup(x => x.Logout(RequestDto))
                        .Throws(new Exception("Invalid Token"));

            var Result = _authController.Logout(RequestDto);

            Assert.That(Result.IsSuccess, Is.False);
            Assert.That("Invalid Token", Is.EqualTo(Result.Message));
        }

        [Test]
        public void Refresh_ValidToken_ReturnsSuccess()
        {
            var RequestDto = new RefreshTokenDto
            {
                RefreshToken = "RefreshToken"
            };

            var Response = new JWTResponseDto
            {
                AccessToken = "NewAccessToken",
                RefreshToken = "RefreshToken"
            };

            _authService.Setup(x => x.RefreshAccessToken(RequestDto.RefreshToken))
                        .Returns(Response);

            var Result = _authController.Refresh(RequestDto);

            Assert.That(Result.IsSuccess, Is.True);
            Assert.That(Response, Is.EqualTo(Result.Data));
        }

        [Test]
        public void Refresh_ServiceThrows_ThrowsException()
        {
            var RequestDto = new RefreshTokenDto
            {
                RefreshToken = "RefreshToken"
            };

            _authService.Setup(x => x.RefreshAccessToken(RequestDto.RefreshToken))
                        .Throws(new Exception("Invalid Refresh Token"));

            Action action = () => _authController.Refresh(RequestDto);

            var Exception = Assert.Throws<Exception>(action);

            Assert.That(Exception.Message, Is.EqualTo("Invalid Refresh Token"));
        }


    }
}
