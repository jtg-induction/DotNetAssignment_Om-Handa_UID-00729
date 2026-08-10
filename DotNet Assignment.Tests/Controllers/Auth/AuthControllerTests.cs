using DotNet_Assignment.Controllers;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Services.Auth;
using DotNet_Assignment.Tests.Utils;
using Moq;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Results;

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
        public async Task SignUp_ValidRequest_ReturnsSuccess()
        {
            var requestDto = AuthTestUtil.CreateMockSignupRequestDto();

            var response = new JWTResponseDto
            {
                AccessToken = "AccessToken",
                RefreshToken = "RefreshToken"
            };

            _authService.Setup(x => x.RegisterAsync(requestDto))
                        .ReturnsAsync(response);

            var result = await _authController.SignUpAsync(requestDto);

            var okResult = result as OkNegotiatedContentResult<ApiResponseDto<JWTResponseDto>>;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Content.IsSuccess, Is.True);
            Assert.That(okResult.Content.Message, Is.EqualTo("User signed in"));
            Assert.That(okResult.Content.Data, Is.EqualTo(response));
        }

        [Test]
        public async Task SignUp_ServiceThrows_ReturnsFailure()
        {
            var requestDto = AuthTestUtil.CreateMockSignupRequestDto();

            _authService.Setup(x => x.RegisterAsync(requestDto))
                        .ThrowsAsync(new Exception("User Already Exists"));

            Func<Task> Action = async () => await _authController.SignUpAsync(requestDto);

            var exception = Assert.CatchAsync<Exception>(Action);

            Assert.That(exception.Message, Is.EqualTo("User Already Exists"));
        }

        [Test]
        public async Task Login_ValidRequest_ReturnsSuccess()
        {
            var requestDto = AuthTestUtil.CreateMockLoginRequestDto();

            var response = new JWTResponseDto
            {
                AccessToken = "AccessToken",
                RefreshToken = "RefreshToken"
            };

            _authService.Setup(x => x.LoginAsync(requestDto))
                        .ReturnsAsync(response);

            var result = await _authController.LoginAsync(requestDto);

            var okResult = result as OkNegotiatedContentResult<ApiResponseDto<JWTResponseDto>>;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Content.IsSuccess, Is.True);
            Assert.That(okResult.Content.Message, Is.EqualTo("User logged in"));
            Assert.That(okResult.Content.Data, Is.EqualTo(response));
        }

        [Test]
        public async Task Login_ServiceThrows_ReturnsFailure()
        {
            var requestDto = AuthTestUtil.CreateMockLoginRequestDto();

            _authService.Setup(x => x.LoginAsync(requestDto))
                        .ThrowsAsync(new Exception("Invalid Credentials"));


            Func<Task> Action = async () => await _authController.LoginAsync(requestDto);

            var exception = Assert.CatchAsync<Exception>(Action);

            Assert.That(exception.Message, Is.EqualTo("Invalid Credentials"));
        }

        [Test]
        public async Task Logout_ValidRequest_ReturnsSuccess()
        {
            var requestDto = new LogoutRequestDto
            {
                RefreshToken = "RefreshToken"
            };

            var result = await _authController.LogoutAsync(requestDto);

            var okResult = result as OkNegotiatedContentResult<ApiResponseDto<object>>;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Content.IsSuccess, Is.True);
            Assert.That(okResult.Content.Message, Is.EqualTo("User logged out"));

            _authService.Verify(x => x.LogoutAsync(requestDto), Times.Once);
        }

        [Test]
        public async Task Logout_ServiceThrows_ReturnsFailure()
        {
            var requestDto = new LogoutRequestDto
            {
                RefreshToken = "RefreshToken"
            };

            _authService.Setup(x => x.LogoutAsync(requestDto))
                        .ThrowsAsync(new Exception("Invalid Token"));

            Func<Task> Action = async()=> await _authController.LogoutAsync(requestDto);

            var exception = Assert.CatchAsync<Exception>(Action);

            Assert.That(exception.Message, Is.EqualTo("Invalid Token"));
        }

        [Test]
        public async Task Refresh_ValidToken_ReturnsSuccess()
        {
            var requestDto = new RefreshTokenDto
            {
                RefreshToken = "RefreshToken"
            };

            var response = new JWTResponseDto
            {
                AccessToken = "NewAccessToken",
                RefreshToken = "RefreshToken"
            };

            _authService.Setup(x => x.RefreshAccessTokenAsync(requestDto.RefreshToken))
                        .ReturnsAsync(response);

            var result = await _authController.RefreshAsync(requestDto);

            var okResult = result as OkNegotiatedContentResult<ApiResponseDto<JWTResponseDto>>;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Content.IsSuccess, Is.True);
            Assert.That(okResult.Content.Message, Is.EqualTo("New token generated"));
            Assert.That(okResult.Content.Data, Is.EqualTo(response));
        }

        [Test]
        public void Refresh_ServiceThrows_ThrowsException()
        {
            var requestDto = new RefreshTokenDto
            {
                RefreshToken = "RefreshToken"
            };

            _authService.Setup(x => x.RefreshAccessTokenAsync(requestDto.RefreshToken))
                        .ThrowsAsync(new Exception("Invalid Refresh Token"));

            Func<Task> Action = async () => await _authController.RefreshAsync(requestDto);

            var exception = Assert.CatchAsync<Exception>(Action);

            Assert.That(exception.Message, Is.EqualTo("Invalid Refresh Token"));
        }
    }
}