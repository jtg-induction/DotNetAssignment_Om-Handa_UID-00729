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
            if (!ModelState.IsValid)
            {
                var Errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();

                var Response = new ApiResponseDto<object>() { IsSuccess = false, Message = "Invalid Credentials", Data = Errors };

                return Content(HttpStatusCode.BadRequest, Response);
            }

            try
            {
                JWTResponseDto JWTResponse= await _authService.RegisterAsync(requestDto);
                var Response = new ApiResponseDto<JWTResponseDto>() { IsSuccess = true, Message = "User Signed In", Data = JWTResponse };
                return Ok(Response);
            }
            catch(Exception ex)
            {
                var Response = new ApiResponseDto<object>() { IsSuccess = false, Message = ex.Message };
                return Content(HttpStatusCode.BadRequest, Response);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> LoginAsync(LoginRequestDto requestDto) {

            if (!ModelState.IsValid)
            {
                var Errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();

                var Response = new ApiResponseDto<object>() { IsSuccess = false, Message = "Invalid Credentials", Data = Errors };

                return Content(HttpStatusCode.BadRequest, Response);
            }

            try
            {
                JWTResponseDto JWTResponse = await _authService.LoginAsync(requestDto);
                var Response = new ApiResponseDto<JWTResponseDto>() { IsSuccess = true, Message = "User Logged In", Data = JWTResponse };
                return Ok(Response);
            }
            catch (Exception ex)
            {
                var Response = new ApiResponseDto<JWTResponseDto>() { IsSuccess = false, Message = ex.Message };
                return Content(HttpStatusCode.BadRequest, Response);
            }
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IHttpActionResult> LogoutAsync(LogoutRequestDto requestDto)
        {
            try
            {
                await _authService.LogoutAsync(requestDto);

                var Response=  new ApiResponseDto<object>() { IsSuccess = true, Message = "User Logged Out"};
                return Ok(Response);
            }
            catch (Exception ex)
            {
                var Response = new ApiResponseDto<object>() { IsSuccess = false, Message = ex.Message };
                return Content(HttpStatusCode.BadRequest, Response);
            }
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IHttpActionResult> RefreshAsync(RefreshTokenDto refreshTokenDto)
        {
            JWTResponseDto JWTResponse = await _authService.RefreshAccessTokenAsync(refreshTokenDto.RefreshToken);

            var Response = new ApiResponseDto<JWTResponseDto>() { IsSuccess = true, Message = "New Token Generated", Data = JWTResponse };
            return Ok(Response);
        }

    }
}
