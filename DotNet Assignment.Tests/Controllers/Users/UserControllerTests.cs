using DotNet_Assignment.Controllers;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Services.Auth;
using DotNet_Assignment.Services.Users;
using DotNet_Assignment.Tests.Utils;
using Moq;
using NUnit.Framework;
using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Results;

namespace DotNet_Assignment.Tests.Controllers.Users
{
    [TestFixture]
    public class UserControllerTests
    {
        private Mock<IUserService> _userService;
        private UserController _userController;

        [SetUp]
        public void Setup()
        {
            _userService = new Mock<IUserService>();

            _userController = new UserController(_userService.Object);
        }

        [Test]
        public async Task UpdateUser_ValidRequest_ReturnsSuccess()
        {
            var userId = Guid.NewGuid();

            SetUser(userId);

            var dto = UserTestUtil.CreateMockUpdateUserDto();

            var Result = await _userController.UpdateUserAsync(dto);

            var OkResult = Result as OkNegotiatedContentResult<ApiResponseDto<object>>;

            Assert.That(OkResult, Is.Not.Null);
            Assert.That(OkResult.Content.IsSuccess, Is.True);
            Assert.That(OkResult.Content.Message, Is.EqualTo("Profile Updated"));

            _userService.Verify(x => x.UpdateUserAsync(userId, dto), Times.Once);
        }

        [Test]
        public async Task UpdateUser_ServiceThrows_ReturnsFailure()
        {
            var userId = Guid.NewGuid();

            SetUser(userId);

            var dto = UserTestUtil.CreateMockUpdateUserDto();

            _userService.Setup(x => x.UpdateUserAsync(userId, dto))
                        .ThrowsAsync(new Exception("User Not Found"));

            var Result = await _userController.UpdateUserAsync(dto);

            var BadRequest = Result as NegotiatedContentResult<ApiResponseDto<object>>;

            Assert.That(BadRequest, Is.Not.Null);
            Assert.That(BadRequest.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(BadRequest.Content.IsSuccess, Is.False);
            Assert.That(BadRequest.Content.Message, Is.EqualTo("User Not Found"));
        }

        [Test]
        public async Task UpdateUser_InvalidModelState_ReturnsFailure()
        {
            var userId = Guid.NewGuid();

            SetUser(userId);

            var dto = UserTestUtil.CreateMockUpdateUserDto();

            _userController.ModelState.AddModelError("Email", "Required");

            var Result = await _userController.UpdateUserAsync(dto);

            var BadRequest = Result as NegotiatedContentResult<ApiResponseDto<object>>;

            Assert.That(BadRequest, Is.Not.Null);
            Assert.That(BadRequest.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(BadRequest.Content.IsSuccess, Is.False);
            Assert.That(BadRequest.Content.Message, Is.EqualTo("Invalid Credentials"));
        }

        [Test]
        public async Task UpdateAddress_ValidRequest_ReturnsSuccess()
        {
            var userId = Guid.NewGuid();
            var addressId = Guid.NewGuid();

            SetUser(userId);

            var dto = UserTestUtil.CreateMockAddressDto();

            var Result = await _userController.UpdateAddressAsync(addressId, dto);

            var OkResult = Result as OkNegotiatedContentResult<ApiResponseDto<object>>;

            Assert.That(OkResult, Is.Not.Null);
            Assert.That(OkResult.Content.IsSuccess, Is.True);
            Assert.That(OkResult.Content.Message, Is.EqualTo("Address Updated Successfully"));

            _userService.Verify(x => x.UpdateUserAddressAsync(userId, addressId, dto), Times.Once);
        }

        [Test]
        public async Task UpdateAddress_ServiceThrows_ReturnsFailure()
        {
            var userId = Guid.NewGuid();
            var addressId = Guid.NewGuid();

            SetUser(userId);

            var dto = UserTestUtil.CreateMockAddressDto();

            _userService.Setup(x => x.UpdateUserAddressAsync(userId, addressId, dto))
                        .ThrowsAsync(new Exception("Address Not Found"));

            var Result = await _userController.UpdateAddressAsync(addressId, dto);

            var BadRequest = Result as NegotiatedContentResult<ApiResponseDto<object>>;

            Assert.That(BadRequest, Is.Not.Null);
            Assert.That(BadRequest.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(BadRequest.Content.IsSuccess, Is.False);
            Assert.That(BadRequest.Content.Message, Is.EqualTo("Address Not Found"));
        }
        [Test]
        public async Task UpdateAddress_InvalidModelState_ReturnsFailure()
        {
            var userId = Guid.NewGuid();
            var addressId = Guid.NewGuid();

            SetUser(userId);

            var dto = UserTestUtil.CreateMockAddressDto();

            _userController.ModelState.AddModelError("City", "Required");

            var Result = await _userController.UpdateAddressAsync(addressId, dto);

            var BadRequest = Result as NegotiatedContentResult<ApiResponseDto<object>>;

            Assert.That(BadRequest, Is.Not.Null);
            Assert.That(BadRequest.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(BadRequest.Content.IsSuccess, Is.False);
            Assert.That(BadRequest.Content.Message, Is.EqualTo("Invalid Credentials"));
        }

        [Test]
        public async Task DeactivateUser_ValidRequest_ReturnsSuccess()
        {
            var userId = Guid.NewGuid();

            SetUser(userId);

            var Result = await _userController.DeactivateUserAsync();

            var OkResult = Result as OkNegotiatedContentResult<ApiResponseDto<object>>;

            Assert.That(OkResult, Is.Not.Null);
            Assert.That(OkResult.Content.IsSuccess, Is.True);
            Assert.That(OkResult.Content.Message, Is.EqualTo("User Deactivated"));

            _userService.Verify(x => x.DeactivateUserAsync(userId), Times.Once);
        }

        [Test]
        public async Task DeactivateUser_ServiceThrows_ReturnsFailure()
        {
            var userId = Guid.NewGuid();

            SetUser(userId);

            _userService.Setup(x => x.DeactivateUserAsync(userId))
                        .ThrowsAsync(new Exception("User Not Found"));

            var Result = await _userController.DeactivateUserAsync();

            var BadRequest = Result as NegotiatedContentResult<ApiResponseDto<object>>;

            Assert.That(BadRequest, Is.Not.Null);
            Assert.That(BadRequest.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(BadRequest.Content.IsSuccess, Is.False);
            Assert.That(BadRequest.Content.Message, Is.EqualTo("User Not Found"));
        }

        [Test]
        public async Task AddAddress_ValidRequest_ReturnsSuccess()
        {
            var userId = Guid.NewGuid();

            SetUser(userId);

            var dto = UserTestUtil.CreateMockAddressDto();

            var Result = await _userController.AddAddressAsync(dto);

            var OkResult = Result as OkNegotiatedContentResult<ApiResponseDto<object>>;

            Assert.That(OkResult, Is.Not.Null);
            Assert.That(OkResult.Content.IsSuccess, Is.True);
            Assert.That(OkResult.Content.Message, Is.EqualTo("Address Added Successfully"));

            _userService.Verify(x => x.AddUserAddressAsync(userId, dto), Times.Once);
        }

        [Test]
        public async Task AddAddress_ServiceThrows_ReturnsFailure()
        {
            var userId = Guid.NewGuid();

            SetUser(userId);

            var dto = UserTestUtil.CreateMockAddressDto();

            _userService.Setup(x => x.AddUserAddressAsync(userId, dto))
                        .ThrowsAsync(new Exception("Unable To Add Address"));

            var Result = await _userController.AddAddressAsync(dto);

            var BadRequest = Result as NegotiatedContentResult<ApiResponseDto<object>>;

            Assert.That(BadRequest, Is.Not.Null);
            Assert.That(BadRequest.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(BadRequest.Content.IsSuccess, Is.False);
            Assert.That(BadRequest.Content.Message, Is.EqualTo("Unable To Add Address"));
        }

        private void SetUser(Guid userId)
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim("UserID", userId.ToString())
            });

            _userController.ControllerContext = new HttpControllerContext();
            _userController.User = new ClaimsPrincipal(identity);
        }
    }
}
