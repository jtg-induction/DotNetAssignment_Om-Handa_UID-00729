using DotNet_Assignment.Constants;
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

        /// <summary>
        /// Updates user profile- name and phone number
        /// </summary>
        /// <param name="updateUserDto">name or/and phone number to be updated</param>
        /// <returns>Https status code including ApiResponseDto with success message</returns>
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
                Message = SuccessMessages.ProfileUpdated
            };

            return Ok(response);
        }

        /// <summary>
        /// Deactivates a user profile
        /// </summary>
        /// <returns>Https status code including ApiResponseDto with success message</returns>
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
                Message = SuccessMessages.UserDeactivated
            };

            return Ok(response);
        }

        /// <summary>
        /// Changes users password
        /// </summary>
        /// <param name="changePasswordDto">Users Old and new Password</param>
        /// <returns>Https status code including ApiResponseDto with success message</returns>
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
                Message = SuccessMessages.PasswordChanged
            };

            return Ok(response);
        }
    }
}
