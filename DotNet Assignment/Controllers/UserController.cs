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

            if (!ModelState.IsValid)
            {
                var Errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();

                var Response = new ApiResponseDto<object>() { IsSuccess = false, Message = "Invalid Credentials", Data = Errors };

                return Content(HttpStatusCode.BadRequest, Response);
            }

            try
            {
                var UserId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst("UserID").Value);

                await _userService.UpdateUserAsync(UserId, updateUserDto);

                var Response = new ApiResponseDto<object>()
                {
                    IsSuccess = true,
                    Message = "Profile Updated"
                };

                return Ok(Response);
            }
            catch (Exception ex)
            {

                var Response = new ApiResponseDto<object>()
                {
                    IsSuccess = false,
                    Message = ex.Message
                };

                return Content(HttpStatusCode.BadRequest, Response);
            }
        }

        [Authorize]
        [HttpPatch]
        [Route("address/{addressId:guid}")]
        public async Task<IHttpActionResult> UpdateAddressAsync(Guid addressId, AddressDto addressDto)
        {
            if (!ModelState.IsValid)
            {
                var Errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();

                var Response = new ApiResponseDto<object>() { IsSuccess = false, Message = "Invalid Credentials", Data = Errors };

                return Content(HttpStatusCode.BadRequest, Response);
            }

            try
            {
                var UserId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst("UserID").Value);

                await _userService.UpdateUserAddressAsync(UserId, addressId, addressDto);

                var Response = new ApiResponseDto<object>()
                {
                    IsSuccess = true,
                    Message = "Address Updated Successfully"
                };

                return Ok(Response);
            }
            catch (Exception ex) {

                var Response = new ApiResponseDto<object>()
                {
                    IsSuccess = false,
                    Message = ex.Message
                };

                return Content(HttpStatusCode.BadRequest, Response);
            }
        }

        [Authorize]
        [HttpPatch]
        [Route("deactivate")]
        public async Task<IHttpActionResult> DeactivateUserAsync()
        {
           try
            {
                var UserId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst("UserID").Value);

                await _userService.DeactivateUserAsync(UserId);

                var Response = new ApiResponseDto<object>()
                {
                    IsSuccess = true,
                    Message = "User Deactivated"
                };

                return Ok(Response);
           }
           catch (Exception ex)
           {
                var Response = new ApiResponseDto<object>()
                {
                    IsSuccess = false,
                    Message = ex.Message
                };

                return Content(HttpStatusCode.BadRequest, Response);
           }
        }

        [Authorize]
        [HttpPost]
        [Route("address")]
        public async Task<IHttpActionResult> AddAddressAsync(AddressDto addressDto)
        {
            try
            {
                var UserId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst("UserID").Value);

                await _userService.AddUserAddressAsync(UserId, addressDto);

                var Response = new ApiResponseDto<object>()
                {
                    IsSuccess = true,
                    Message = "Address Added Successfully"
                };

                return Ok(Response);
            }
            catch (Exception ex)
            {
                var Response = new ApiResponseDto<object>()
                {
                    IsSuccess = false,
                    Message = ex.Message
                };

                return Content(HttpStatusCode.BadRequest, Response);
            }
        }
    }
}
