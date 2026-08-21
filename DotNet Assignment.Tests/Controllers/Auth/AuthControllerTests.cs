using DotNet_Assignment.Constants;
using DotNet_Assignment.Controllers;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Services.Auth;
using DotNet_Assignment.Tests.Constants;
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

        /// <summary>
        /// Signup action - Sends Valid Request - Successfully signs up and returns success
        /// </summary>
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
            Assert.That(okResult.Content.Message, Is.EqualTo(SuccessMessages.UserSignedIn));
            Assert.That(okResult.Content.Data, Is.EqualTo(response));
        }

        /// <summary>
        /// Signup action - Service Throws Exception - returns Exception
        /// </summary>
        [Test]
        public async Task SignUp_ServiceThrows_ReturnsFailure()
        {
            var requestDto = AuthTestUtil.CreateMockSignupRequestDto();

            _authService.Setup(x => x.RegisterAsync(requestDto))
                        .ThrowsAsync(new Exception(ExceptionMessages.EmailAlreadyExists));

            Func<Task> action = async () => await _authController.SignUpAsync(requestDto);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.EmailAlreadyExists));
        }

        /// <summary>
        /// Login action - Sends Valid Request - Successfully logs in and returns success
        /// </summary>
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
            Assert.That(okResult.Content.Message, Is.EqualTo(SuccessMessages.UserLoggedIn));
            Assert.That(okResult.Content.Data, Is.EqualTo(response));
        }

        /// <summary>
        /// Login action - Service Throws Exception - returns Exception
        /// </summary>
        [Test]
        public async Task Login_ServiceThrows_ReturnsFailure()
        {
            var requestDto = AuthTestUtil.CreateMockLoginRequestDto();

            _authService.Setup(x => x.LoginAsync(requestDto))
                        .ThrowsAsync(new Exception(ExceptionMessages.InvalidCredentials));


            Func<Task> action = async () => await _authController.LoginAsync(requestDto);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.InvalidCredentials));
        }

        /// <summary>
        /// Logout action - Sends Valid Request - Successfully logs out and returns success
        /// </summary>
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
            Assert.That(okResult.Content.Message, Is.EqualTo(SuccessMessages.UserLoggedOut));

            _authService.Verify(x => x.LogoutAsync(requestDto), Times.Once);
        }

        /// <summary>
        /// Signup action - Service Throws Exception - returns Exception
        /// </summary>
        [Test]
        public async Task Logout_ServiceThrows_ReturnsFailure()
        {
            var requestDto = new LogoutRequestDto
            {
                RefreshToken = "RefreshToken"
            };

            _authService.Setup(x => x.LogoutAsync(requestDto))
                        .ThrowsAsync(new Exception(ExceptionMessages.RefreshTokenInvalid));

            Func<Task> action = async()=> await _authController.LogoutAsync(requestDto);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.RefreshTokenInvalid));
        }

        /// <summary>
        /// Refresh action - Sends Valid Request - Successfully refreshes access token and returns success
        /// </summary>
        [Test]
        public async Task Refresh_ValidToken_ReturnsSuccess()
        {
            var requestDto = new RefreshTokenDto
            {
                RefreshToken = Constants.MockConstants.MockRefreshToken
            };

            var response = new JWTResponseDto
            {
                AccessToken = Constants.MockConstants.MockNewAccessToken,
                RefreshToken = Constants.MockConstants.MockRefreshToken
            };

            _authService.Setup(x => x.RefreshAccessTokenAsync(requestDto.RefreshToken))
                        .ReturnsAsync(response);

            var result = await _authController.RefreshAsync(requestDto);

            var okResult = result as OkNegotiatedContentResult<ApiResponseDto<JWTResponseDto>>;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Content.IsSuccess, Is.True);
            Assert.That(okResult.Content.Message, Is.EqualTo(SuccessMessages.NewTokenGenerated));
            Assert.That(okResult.Content.Data, Is.EqualTo(response));
        }

        /// <summary>
        /// Refresh action - Service Throws Exception - returns Exception
        /// </summary>
        [Test]
        public void Refresh_ServiceThrows_ThrowsException()
        {
            var requestDto = new RefreshTokenDto
            {
                RefreshToken = Constants.MockConstants.MockRefreshToken
            };

            _authService.Setup(x => x.RefreshAccessTokenAsync(requestDto.RefreshToken))
                        .ThrowsAsync(new Exception(ExceptionMessages.RefreshTokenInvalid));

            Func<Task> action = async () => await _authController.RefreshAsync(requestDto);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.RefreshTokenInvalid));
        }
    }
}
