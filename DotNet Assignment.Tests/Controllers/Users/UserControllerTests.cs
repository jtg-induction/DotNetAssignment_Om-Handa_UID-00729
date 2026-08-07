using DotNet_Assignment.Controllers;
using DotNet_Assignment.Services.Auth;
using DotNet_Assignment.Services.Users;
using DotNet_Assignment.Tests.Utils;
using Moq;
using NUnit.Framework;
using System;

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
        public void UpdateUser_ValidRequest_ReturnsSuccess()
        {
            var userId = Guid.NewGuid();
            SetUser(userId);

            var dto = UserTestUtil.CreateMockUpdateUserDto();

            var result = _userController.UpdateUser(dto);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That("Profile Updated", Is.EqualTo(result.Message));

            _userService.Verify(x => x.UpdateUser(userId, dto), Times.Once);
        }

        [Test]
        public void UpdateAddress_ValidRequest_ReturnsSuccess()
        {
            var userId = Guid.NewGuid();
            var addressId = Guid.NewGuid();

            SetUser(userId);

            var dto = UserTestUtil.CreateMockUpdateAddressDto();

            var result = _userController.UpdateAddress(addressId, dto);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That("Address Updated Successfully", Is.EqualTo(result.Message));

            _userService.Verify(x => x.UpdateUserAddress(userId, addressId, dto), Times.Once);
        }

        [Test]
        public void DeactivateUser_ValidRequest_ReturnsSuccess()
        {
            var userId = Guid.NewGuid();

            SetUser(userId);

            var result = _userController.DeactivateUser();

            Assert.That(result.IsSuccess, Is.True);
            Assert.That("User Deactivated", Is.EqualTo(result.Message));

            _userService.Verify(x => x.DeactivateUser(userId), Times.Once);
        }
    }
}
}
