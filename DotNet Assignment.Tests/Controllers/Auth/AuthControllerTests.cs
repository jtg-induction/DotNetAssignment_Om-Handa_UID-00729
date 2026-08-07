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
            var RequestDto = AuthTestUtil.CreateMockSignupRequestDto();

            var Response = new JWTResponseDto
            {
                AccessToken = "AccessToken",
                RefreshToken = "RefreshToken"
            };

            _authService.Setup(x => x.RegisterAsync(RequestDto))
                        .ReturnsAsync(Response);

            var Result = await _authController.SignUpAsync(RequestDto);

            var OkResult = Result as OkNegotiatedContentResult<ApiResponseDto<JWTResponseDto>>;

            Assert.That(OkResult, Is.Not.Null);
            Assert.That(OkResult.Content.IsSuccess, Is.True);
            Assert.That(OkResult.Content.Message, Is.EqualTo("User Signed In"));
            Assert.That(OkResult.Content.Data, Is.EqualTo(Response));
        }

        [Test]
        public async Task SignUp_ServiceThrows_ReturnsFailure()
        {
            var RequestDto = AuthTestUtil.CreateMockSignupRequestDto();

            _authService.Setup(x => x.RegisterAsync(RequestDto))
                        .ThrowsAsync(new Exception("User Already Exists"));

            var Result = await _authController.SignUpAsync(RequestDto);

            var BadRequest = Result as NegotiatedContentResult<ApiResponseDto<object>>;

            Assert.That(BadRequest, Is.Not.Null);
            Assert.That(BadRequest.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(BadRequest.Content.IsSuccess, Is.False);
            Assert.That(BadRequest.Content.Message, Is.EqualTo("User Already Exists"));
        }

        [Test]
        public async Task SignUp_InvalidModelState_ReturnsFailure()
        {
            var RequestDto = AuthTestUtil.CreateMockSignupRequestDto();

            _authController.ModelState.AddModelError("Email", "Required");

            var Result = await _authController.SignUpAsync(RequestDto);

            var BadRequest = Result as NegotiatedContentResult<ApiResponseDto<object>>;

            Assert.That(BadRequest, Is.Not.Null);
            Assert.That(BadRequest.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(BadRequest.Content.IsSuccess, Is.False);
            Assert.That(BadRequest.Content.Message, Is.EqualTo("Invalid Credentials"));
        }

        [Test]
        public async Task Login_ValidRequest_ReturnsSuccess()
        {
            var RequestDto = AuthTestUtil.CreateMockLoginRequestDto();

            var Response = new JWTResponseDto
            {
                AccessToken = "AccessToken",
                RefreshToken = "RefreshToken"
            };

            _authService.Setup(x => x.LoginAsync(RequestDto))
                        .ReturnsAsync(Response);

            var Result = await _authController.LoginAsync(RequestDto);

            var OkResult = Result as OkNegotiatedContentResult<ApiResponseDto<JWTResponseDto>>;

            Assert.That(OkResult, Is.Not.Null);
            Assert.That(OkResult.Content.IsSuccess, Is.True);
            Assert.That(OkResult.Content.Message, Is.EqualTo("User Logged In"));
            Assert.That(OkResult.Content.Data, Is.EqualTo(Response));
        }

        [Test]
        public async Task Login_ServiceThrows_ReturnsFailure()
        {
            var RequestDto = AuthTestUtil.CreateMockLoginRequestDto();

            _authService.Setup(x => x.LoginAsync(RequestDto))
                        .ThrowsAsync(new Exception("Invalid Credentials"));

            var Result = await _authController.LoginAsync(RequestDto);

            var BadRequest = Result as NegotiatedContentResult<ApiResponseDto<JWTResponseDto>>;

            Assert.That(BadRequest, Is.Not.Null);
            Assert.That(BadRequest.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(BadRequest.Content.IsSuccess, Is.False);
            Assert.That(BadRequest.Content.Message, Is.EqualTo("Invalid Credentials"));
        }

        [Test]
        public async Task Login_InvalidModelState_ReturnsFailure()
        {
            var RequestDto = AuthTestUtil.CreateMockLoginRequestDto();

            _authController.ModelState.AddModelError("Email", "Required");

            var Result = await _authController.LoginAsync(RequestDto);

            var BadRequest = Result as NegotiatedContentResult<ApiResponseDto<object>>;

            Assert.That(BadRequest, Is.Not.Null);
            Assert.That(BadRequest.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(BadRequest.Content.IsSuccess, Is.False);
            Assert.That(BadRequest.Content.Message, Is.EqualTo("Invalid Credentials"));
        }

        [Test]
        public async Task Logout_ValidRequest_ReturnsSuccess()
        {
            var RequestDto = new LogoutRequestDto
            {
                RefreshToken = "RefreshToken"
            };

            var Result = await _authController.LogoutAsync(RequestDto);

            var OkResult = Result as OkNegotiatedContentResult<ApiResponseDto<object>>;

            Assert.That(OkResult, Is.Not.Null);
            Assert.That(OkResult.Content.IsSuccess, Is.True);
            Assert.That(OkResult.Content.Message, Is.EqualTo("User Logged Out"));

            _authService.Verify(x => x.LogoutAsync(RequestDto), Times.Once);
        }

        [Test]
        public async Task Logout_ServiceThrows_ReturnsFailure()
        {
            var RequestDto = new LogoutRequestDto
            {
                RefreshToken = "RefreshToken"
            };

            _authService.Setup(x => x.LogoutAsync(RequestDto))
                        .ThrowsAsync(new Exception("Invalid Token"));

            var Result = await _authController.LogoutAsync(RequestDto);

            var BadRequest = Result as NegotiatedContentResult<ApiResponseDto<object>>;

            Assert.That(BadRequest, Is.Not.Null);
            Assert.That(BadRequest.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(BadRequest.Content.IsSuccess, Is.False);
            Assert.That(BadRequest.Content.Message, Is.EqualTo("Invalid Token"));
        }

        [Test]
        public async Task Refresh_ValidToken_ReturnsSuccess()
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

            _authService.Setup(x => x.RefreshAccessTokenAsync(RequestDto.RefreshToken))
                        .ReturnsAsync(Response);

            var Result = await _authController.RefreshAsync(RequestDto);

            var OkResult = Result as OkNegotiatedContentResult<ApiResponseDto<JWTResponseDto>>;

            Assert.That(OkResult, Is.Not.Null);
            Assert.That(OkResult.Content.IsSuccess, Is.True);
            Assert.That(OkResult.Content.Message, Is.EqualTo("New Token Generated"));
            Assert.That(OkResult.Content.Data, Is.EqualTo(Response));
        }

        [Test]
        public void Refresh_ServiceThrows_ThrowsException()
        {
            var RequestDto = new RefreshTokenDto
            {
                RefreshToken = "RefreshToken"
            };

            _authService.Setup(x => x.RefreshAccessTokenAsync(RequestDto.RefreshToken))
                        .ThrowsAsync(new Exception("Invalid Refresh Token"));

            var Exception = Assert.ThrowsAsync<Exception>((AsyncTestDelegate)(async () =>
                await _authController.RefreshAsync(RequestDto)));

            Assert.That(Exception.Message, Is.EqualTo("Invalid Refresh Token"));
        }
    }
}