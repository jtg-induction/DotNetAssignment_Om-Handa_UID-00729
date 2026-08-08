using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Services.Users;
using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace DotNet_Assignment.Controllers
{
    [RoutePrefix("api/user")]

    public class UserController : ApiController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpPatch]
        [Route("update")]
        public async Task<IHttpActionResult> UpdateUserAsync(UpdateUserDto updateUserDto)
        {
             var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);

             await _userService.UpdateUserAsync(userId, updateUserDto);

             var response = new ApiResponseDto<object>()
             {
                  IsSuccess = true,
                  Message = "Profile Updated"
             };

             return Ok(response);
        }

        [Authorize]
        [HttpPatch]
        [Route("deactivate")]
        public async Task<IHttpActionResult> DeactivateUserAsync()
        {
                var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);

                await _userService.DeactivateUserAsync(userId);

                var response = new ApiResponseDto<object>()
                {
                    IsSuccess = true,
                    Message = "User Deactivated"
                };

                return Ok(response);
        }

        [Authorize]
        [HttpPatch]
        [Route("change-password")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);

            await _userService.ChangePasswordAsync(userId, changePasswordDto);

            var response = new ApiResponseDto<object>()
            {
                IsSuccess = true,
                Message = "Password Changed"
            };

            return Ok(response);
        }
    }
}
