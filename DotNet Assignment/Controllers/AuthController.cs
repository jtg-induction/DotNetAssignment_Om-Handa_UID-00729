using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Services.Auth;
using System;
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
        public ApiResponseDto<JWTResponseDto> SignUp(SignupRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception("Wrong Details");
            }

            try
            {
                JWTResponseDto JWTResponse= _authService.Register(requestDto);
                return new ApiResponseDto<JWTResponseDto>() { IsSuccess=true, Message="User Signed In", Data = JWTResponse } ;
            }
            catch(Exception ex)
            {
                return new ApiResponseDto<JWTResponseDto>() { IsSuccess=false, Message=ex.Message };
            }
        }

        [HttpPost]
        [Route("login")]
        public ApiResponseDto<JWTResponseDto> Login(LoginRequestDto requestDto) {

            if (!ModelState.IsValid)
            {
                throw new Exception("Wrong Details");
            }

            try
            {
                JWTResponseDto JWTResponse = _authService.Login(requestDto);
                return new ApiResponseDto<JWTResponseDto>() { IsSuccess = true, Message = "User Logged In", Data = JWTResponse };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<JWTResponseDto>() { IsSuccess = false, Message = ex.Message };
            }
        }

        [HttpPost]
        [Route("logout")]
        public ApiResponseDto<object> Logout(LogoutRequestDto requestDto)
        {
            try
            {
                _authService.Logout(requestDto);
                return new ApiResponseDto<object>() { IsSuccess = true, Message = "User Logged Out"};
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<object>() { IsSuccess = false, Message = ex.Message };
            }
        }

        [HttpPost]
        [Route("refresh")]
        public ApiResponseDto<JWTResponseDto> Refresh(RefreshTokenDto refreshTokenDto)
        {
            JWTResponseDto JWTResponse = _authService.RefreshAccessToken(refreshTokenDto.RefreshToken);

            return new ApiResponseDto<JWTResponseDto>() { IsSuccess = true, Message = "User Logged In", Data = JWTResponse };
        }

    }
}
