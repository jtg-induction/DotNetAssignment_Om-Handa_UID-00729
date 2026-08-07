
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Repository.RefreshTokens;
using DotNet_Assignment.Repository.Users;
using DotNet_Assignment.Services.Users;
using DotNet_Assignment.Tests.Utils;
using Moq;
using NUnit.Framework;
using System;

namespace DotNet_Assignment.Tests.Services.Users
{
    [TestFixture]
    public class UpdateAddressTests
    {
        private Mock<IUserRepository> _userRepository;
        private Mock<IRefreshTokenRepository> _refreshTokenRepository;
        private UserService _userService;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _refreshTokenRepository = new Mock<IRefreshTokenRepository>();

            _userService = new UserService(
                _userRepository.Object,
                _refreshTokenRepository.Object
            );
        }

        [Test]
        public void UpdateAddress_InvalidId_ThrowsException()
        {
            var RequestDto = UserTestUtil.CreateMockUpdateAddressDto();

            var UserAddressId = Guid.NewGuid();

            var UserId = Guid.NewGuid();

            _userRepository
                .Setup(x => x.GetAddressById(UserAddressId, UserId))
                .Returns((UserAddress)null);

            Action action = () => _userService.UpdateUserAddress(UserId,UserAddressId, RequestDto);

            var Exception = Assert.Throws<Exception>(action);

            Assert.That(Exception.Message, Is.EqualTo("Address not Found"));
        }

        [Test]
        public void UpdateUserAddress_ValidRequest_UpdatesAllFields()
        {
            var userId = Guid.NewGuid();

            var UserAddress = UserTestUtil.CreateMockAddress();

            var UserAddressId = UserAddress.UserAddressId;

            var Request = UserTestUtil.CreateMockUpdateAddressDto();

            _userRepository
                .Setup(x => x.GetAddressById(UserAddressId, userId))
                .Returns(UserAddress);

            _userService.UpdateUserAddress(userId, UserAddressId, Request);

            Assert.That(UserAddress.HouseNumber, Is.EqualTo(Request.HouseNumber));
            Assert.That(UserAddress.Street, Is.EqualTo(Request.Street));
            Assert.That(UserAddress.Landmark, Is.EqualTo(Request.Landmark));
            Assert.That(UserAddress.City, Is.EqualTo(Request.City));
            Assert.That(UserAddress.State, Is.EqualTo(Request.State));
            Assert.That(UserAddress.Pincode, Is.EqualTo(Request.Pincode));
        }

        [Test]
        public void UpdateUserAddress_EmptyHouseNumber_DoesNotUpdate()
        {
            var userId = Guid.NewGuid();

            var UserAddress = UserTestUtil.CreateMockAddress();

            var UserAddressId = UserAddress.UserAddressId;

            var original = UserAddress.HouseNumber;

            var Request = UserTestUtil.CreateMockUpdateAddressDto();
            Request.HouseNumber = "";

            _userRepository
                .Setup(x => x.GetAddressById(UserAddressId, userId))
                .Returns(UserAddress);

            _userService.UpdateUserAddress(userId, UserAddressId, Request);

            Assert.That(UserAddress.HouseNumber, Is.EqualTo(original));
        }

        [Test]
        public void UpdateUserAddress_EmptyStreet_DoesNotUpdate()
        {
            var userId = Guid.NewGuid();

            var UserAddress = UserTestUtil.CreateMockAddress();

            var UserAddressId = UserAddress.UserAddressId;

            var original = UserAddress.Street;

            var Request = UserTestUtil.CreateMockUpdateAddressDto();

            Request.Street = "";

            _userRepository
                .Setup(x => x.GetAddressById(UserAddressId, userId))
                .Returns(UserAddress);

            _userService.UpdateUserAddress(userId, UserAddressId, Request);

            Assert.That(UserAddress.Street, Is.EqualTo(original));
        }

        [Test]
        public void UpdateUserAddress_EmptyLandmark_DoesNotUpdate()
        {
            var userId = Guid.NewGuid();

            var UserAddress = UserTestUtil.CreateMockAddress();

            var UserAddressId = UserAddress.UserAddressId;

            var original = UserAddress.Landmark;

            var Request = UserTestUtil.CreateMockUpdateAddressDto();

            Request.Landmark = "";

            _userRepository
                .Setup(x => x.GetAddressById(UserAddressId, userId))
                .Returns(UserAddress);

            _userService.UpdateUserAddress(userId, UserAddressId, Request);

            Assert.That(UserAddress.Landmark, Is.EqualTo(original));
        }

        [Test]
        public void UpdateUserAddress_EmptyCity_DoesNotUpdate()
        {
            var userId = Guid.NewGuid();

            var UserAddress = UserTestUtil.CreateMockAddress();

            var UserAddressId = UserAddress.UserAddressId;

            var original = UserAddress.City;

            var Request = UserTestUtil.CreateMockUpdateAddressDto();

            Request.City = "";

            _userRepository
                .Setup(x => x.GetAddressById(UserAddressId, userId))
                .Returns(UserAddress);

            _userService.UpdateUserAddress(userId, UserAddressId, Request);

            Assert.That(UserAddress.City, Is.EqualTo(original));
        }

        [Test]
        public void UpdateUserAddress_EmptyState_DoesNotUpdate()
        {
            var userId = Guid.NewGuid();

            var UserAddress = UserTestUtil.CreateMockAddress();

            var UserAddressId = UserAddress.UserAddressId;

            var original = UserAddress.State;

            var Request = UserTestUtil.CreateMockUpdateAddressDto();

            Request.State = "";

            _userRepository
                .Setup(x => x.GetAddressById(UserAddressId, userId))
                .Returns(UserAddress);

            _userService.UpdateUserAddress(userId, UserAddressId, Request);

            Assert.That(UserAddress.State, Is.EqualTo(original));
        }

        [Test]
        public void UpdateUserAddress_EmptyPincode_DoesNotUpdate()
        {
            var userId = Guid.NewGuid();

            var UserAddress = UserTestUtil.CreateMockAddress();

            var UserAddressId = UserAddress.UserAddressId;

            var original = UserAddress.Pincode;

            var Request = UserTestUtil.CreateMockUpdateAddressDto();

            Request.Pincode = "";

            _userRepository
                .Setup(x => x.GetAddressById(UserAddressId, userId))
                .Returns(UserAddress);

            _userService.UpdateUserAddress(userId, UserAddressId, Request);

            Assert.That(UserAddress.Pincode, Is.EqualTo(original));
        }

        [Test]
        public void UpdateUserAddress_ValidRequest_SavesChanges()
        {
            var userId = Guid.NewGuid();

            var UserAddress = UserTestUtil.CreateMockAddress();

            var UserAddressId = UserAddress.UserAddressId;

            var Request = UserTestUtil.CreateMockUpdateAddressDto();

            _userRepository
                .Setup(x => x.GetAddressById(UserAddressId, userId))
                .Returns(UserAddress);

            _userService.UpdateUserAddress(userId, UserAddressId, Request);

            _userRepository.Verify(x => x.Save(), Times.Once);
        }
    }
}
