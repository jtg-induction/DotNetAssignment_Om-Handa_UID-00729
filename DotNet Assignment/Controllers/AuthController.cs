using DotNet_Assignment.Constants;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Services.Auth;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace DotNet_Assignment.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Signs Up a user and generates Auth tokens
        /// </summary>
        /// <param name="requestDto">Signup Details of a user</param>
        /// <returns>Http status code with ApiResponseDto including JWT Response</returns>
        [HttpPost]
        [Route("signup")]
        public async Task<IHttpActionResult> SignUpAsync(SignupRequestDto requestDto)
        {
            JWTResponseDto JWTResponse = await _authService.RegisterAsync(requestDto);
            var response = new ApiResponseDto<JWTResponseDto>() { IsSuccess = true, Message = SuccessMessages.UserSignedIn, Data = JWTResponse };
            return Ok(response);
        }

        /// <summary>
        /// Logs in a User and generates Auth Tokens
        /// </summary>
        /// <param name="requestDto">Email and Password of a User</param>
        /// <returns>Http status code with ApiResponseDto including JWT Response</returns>
        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> LoginAsync(LoginRequestDto requestDto)
        {

            JWTResponseDto JWTResponse = await _authService.LoginAsync(requestDto);
            var response = new ApiResponseDto<JWTResponseDto>() { IsSuccess = true, Message = SuccessMessages.UserLoggedIn, Data = JWTResponse };
            return Ok(response);
        }

        /// <summary>
        /// Logs a user Out and removes refresh tokens
        /// </summary>
        /// <param name="requestDto">Refresh Token to be removed</param>
        /// <returns>Http status code with ApiResponseDto including success message</returns>
        [Authorize]
        [HttpPost]
        [Route("logout")]
        public async Task<IHttpActionResult> LogoutAsync(LogoutRequestDto requestDto)
        {
            await _authService.LogoutAsync(requestDto);

            var response = new ApiResponseDto<object>() { IsSuccess = true, Message = SuccessMessages.UserLoggedOut };
            return Ok(response);
        }

        /// <summary>
        /// Refreshes and generates a new Access Token
        /// </summary>
        /// <param name="refreshTokenDto">Refresh Token</param>
        /// <returns>Http status code with ApiResponseDto including success message</returns>
        [HttpPost]
        [Route("refresh")]
        public async Task<IHttpActionResult> RefreshAsync(RefreshTokenDto refreshTokenDto)
        {
            JWTResponseDto JWTResponse = await _authService.RefreshAccessTokenAsync(refreshTokenDto.RefreshToken);

            var response = new ApiResponseDto<JWTResponseDto>() { IsSuccess = true, Message = SuccessMessages.NewTokenGenerated, Data = JWTResponse };
            return Ok(response);
        }

    }
}
