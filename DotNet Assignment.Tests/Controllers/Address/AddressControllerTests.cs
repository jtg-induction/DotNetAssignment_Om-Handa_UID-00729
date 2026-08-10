using DotNet_Assignment.Controllers;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Services.Address;
using DotNet_Assignment.Services.Auth;
using DotNet_Assignment.Tests.Utils;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Results;

namespace DotNet_Assignment.Tests.Controllers.Address
{
    [TestFixture]
    public class AddressControllerTests
    {
        private Mock<IAddressService> _addressService;
        private AddressController _addressController;

        [SetUp]
        public void Setup()
        {
            _addressService = new Mock<IAddressService>();

            _addressController = new AddressController(_addressService.Object);
        }

        [Test]
        public async Task UpdateAddress_ValidRequest_ReturnsSuccess()
        {
            var userId = Guid.NewGuid();
            var addressId = Guid.NewGuid();

            SetUser(userId);

            var dto = UserTestUtil.CreateMockAddressDto();

            var result = await _addressController.UpdateAddressAsync(addressId, dto);

            var okResult = result as OkNegotiatedContentResult<ApiResponseDto<object>>;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Content.IsSuccess, Is.True);
            Assert.That(okResult.Content.Message, Is.EqualTo("Address updated successfully"));

            _addressService.Verify(x => x.UpdateUserAddressAsync(userId, addressId, dto), Times.Once);
        }

        [Test]
        public async Task UpdateAddress_ServiceThrows_ReturnsFailure()
        {
            var userId = Guid.NewGuid();
            var addressId = Guid.NewGuid();

            SetUser(userId);

            var dto = UserTestUtil.CreateMockAddressDto();

            _addressService.Setup(x => x.UpdateUserAddressAsync(userId, addressId, dto))
                        .ThrowsAsync(new Exception("Address Not Found"));

            Func<Task> Action = async() => await _addressController.UpdateAddressAsync(addressId, dto);

            var exception = Assert.CatchAsync<Exception>(Action);

            Assert.That(exception.Message, Is.EqualTo("Address Not Found"));
        }

        [Test]
        public async Task AddAddress_ValidRequest_ReturnsSuccess()
        {
            var userId = Guid.NewGuid();

            SetUser(userId);

            var dto = UserTestUtil.CreateMockAddressDto();

            var result = await _addressController.AddAddressAsync(dto);

            var okResult = result as OkNegotiatedContentResult<ApiResponseDto<object>>;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Content.IsSuccess, Is.True);
            Assert.That(okResult.Content.Message, Is.EqualTo("Address added successfully"));

            _addressService.Verify(x => x.AddUserAddressAsync(userId, dto), Times.Once);
        }

        [Test]
        public async Task AddAddress_ServiceThrows_ReturnsFailure()
        {
            var userId = Guid.NewGuid();

            SetUser(userId);

            var dto = UserTestUtil.CreateMockAddressDto();

            _addressService.Setup(x => x.AddUserAddressAsync(userId, dto))
                        .ThrowsAsync(new Exception("Unable To Add Address"));

            Func<Task> action = async () => await _addressController.AddAddressAsync(dto);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo("Unable To Add Address"));
        }

        private void SetUser(Guid userId)
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            });

            _addressController.ControllerContext = new HttpControllerContext();
            _addressController.User = new ClaimsPrincipal(identity);
        }
    }
}
