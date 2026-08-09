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

        public AuthController(IAuthService authService) { 
            _authService = authService;
        }

        [HttpPost]
        [Route("signup")]
        public async Task<IHttpActionResult> SignUpAsync(SignupRequestDto requestDto)
        {
            JWTResponseDto JWTResponse= await _authService.RegisterAsync(requestDto);
            var response = new ApiResponseDto<JWTResponseDto>() { IsSuccess = true, Message = "User Signed In", Data = JWTResponse };
            return Ok(response);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> LoginAsync(LoginRequestDto requestDto) {

            JWTResponseDto JWTResponse = await _authService.LoginAsync(requestDto);
            var response = new ApiResponseDto<JWTResponseDto>() { IsSuccess = true, Message = "User Logged In", Data = JWTResponse };
            return Ok(response);
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IHttpActionResult> LogoutAsync(LogoutRequestDto requestDto)
        {
                await _authService.LogoutAsync(requestDto);

                var response=  new ApiResponseDto<object>() { IsSuccess = true, Message = "User Logged Out"};
                return Ok(response);
        }

        [Authorize]
        [HttpPost]
        [Route("refresh")]
        public async Task<IHttpActionResult> RefreshAsync(RefreshTokenDto refreshTokenDto)
        {
            JWTResponseDto JWTResponse = await _authService.RefreshAccessTokenAsync(refreshTokenDto.RefreshToken);

            var response = new ApiResponseDto<JWTResponseDto>() { IsSuccess = true, Message = "New Token Generated", Data = JWTResponse };
            return Ok(response);
        }

    }
}
