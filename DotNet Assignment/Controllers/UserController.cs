using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Services.Auth;
using DotNet_Assignment.Services.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
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
        public ApiResponseDto<object> UpdateUser(UpdateUserDto updateUserDto)
        {
            var UserId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst("UserID").Value);

            _userService.UpdateUser(UserId, updateUserDto);

            return new ApiResponseDto<object>()
            {
                IsSuccess = true,
                Message = "Profile Updated"
            };
        }

        [Authorize]
        [HttpPatch]
        [Route("address/{addressId:guid}")]
        public ApiResponseDto<object> UpdateAddress(Guid addressId, UpdateAddressDto updateAddressDto)
        {
            var UserId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst("UserID").Value);

            _userService.UpdateUserAddress(UserId, addressId, updateAddressDto);

            return new ApiResponseDto<object>()
            {
                IsSuccess = true,
                Message = "Address Updated Successfully"
            };
        }

        [Authorize]
        [HttpPatch]
        [Route("deactivate")]
        public ApiResponseDto<object> DeactivateUser()
        {
            var UserId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst("UserID").Value);

            _userService.DeactivateUser(UserId);

            return new ApiResponseDto<object>()
            {
                IsSuccess = true,
                Message = "User Deactivated"
            };
        }
    }
}
