using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Services.Address;
using DotNet_Assignment.Services.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace DotNet_Assignment.Controllers
{
    [RoutePrefix("api/address")]
    public class AddressController : ApiController
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [Authorize]
        [HttpPost]
        [Route("add")]
        public async Task<IHttpActionResult> AddAddressAsync(AddressDto addressDto)
        {
            var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);

            await _addressService.AddUserAddressAsync(userId, addressDto);

            var response = new ApiResponseDto<object>()
            {
                IsSuccess = true,
                Message = "Address Added Successfully"
            };

            return Ok(response);
        }

        [Authorize]
        [HttpPatch]
        [Route("update/{addressId:guid}")]
        public async Task<IHttpActionResult> UpdateAddressAsync(Guid addressId, AddressDto addressDto)
        {
            var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);

            await _addressService.UpdateUserAddressAsync(userId, addressId, addressDto);

            var response = new ApiResponseDto<object>()
            {
                IsSuccess = true,
                Message = "Address Updated Successfully"
            };

            return Ok(response);
        }

    }
}
