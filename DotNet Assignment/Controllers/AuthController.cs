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

    }
}
