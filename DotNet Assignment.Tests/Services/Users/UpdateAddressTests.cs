
using DotNet_Assignment.Data;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Repository.RefreshTokens;
using DotNet_Assignment.Repository.Users;
using DotNet_Assignment.Services.Users;
using DotNet_Assignment.Tests.Utils;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace DotNet_Assignment.Tests.Services.Users
{
    [TestFixture]
    public class UpdateAddressTests
    {
        private Mock<IUserRepository> _userRepository;
        private Mock<IRefreshTokenRepository> _refreshTokenRepository;
        private Mock<AppDbContext> _appDbContext;
        private UserService _userService;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _refreshTokenRepository = new Mock<IRefreshTokenRepository>();
            _appDbContext = new Mock<AppDbContext>();

            _userService = new UserService(
                _userRepository.Object,
                _refreshTokenRepository.Object,
                _appDbContext.Object
            );
        }

        [Test]
        public async Task UpdateAddress_InvalidId_ThrowsException()
        {
            var RequestDto = UserTestUtil.CreateMockAddressDto();

            var UserAddressId = Guid.NewGuid();

            var UserId = Guid.NewGuid();

            _userRepository
                .Setup(x => x.GetAddressByIdAsync(UserAddressId, UserId))
                .ReturnsAsync((UserAddress)null);

            Func<Task> action = () => _userService.UpdateUserAddressAsync(UserId,UserAddressId, RequestDto);

            var Exception = Assert.CatchAsync<Exception>(action);

            Assert.That(Exception.Message, Is.EqualTo("Address not Found"));
        }

        [Test]
        public async Task UpdateUserAddress_ValidRequest_UpdatesAllFields()
        {
            var userId = Guid.NewGuid();

            var UserAddress = UserTestUtil.CreateMockAddress();

            var UserAddressId = UserAddress.UserAddressId;

            var Request = UserTestUtil.CreateMockAddressDto();

            _userRepository
                .Setup(x => x.GetAddressByIdAsync(UserAddressId, userId))
                .ReturnsAsync(UserAddress);

            await _userService.UpdateUserAddressAsync(userId, UserAddressId, Request);

            Assert.That(UserAddress.HouseNumber, Is.EqualTo(Request.HouseNumber));
            Assert.That(UserAddress.Street, Is.EqualTo(Request.Street));
            Assert.That(UserAddress.Landmark, Is.EqualTo(Request.Landmark));
            Assert.That(UserAddress.City, Is.EqualTo(Request.City));
            Assert.That(UserAddress.State, Is.EqualTo(Request.State));
            Assert.That(UserAddress.Pincode, Is.EqualTo(Request.Pincode));
        }

        [Test]
        public async Task UpdateUserAddress_EmptyHouseNumber_DoesNotUpdate()
        {
            var userId = Guid.NewGuid();

            var UserAddress = UserTestUtil.CreateMockAddress();

            var UserAddressId = UserAddress.UserAddressId;

            var original = UserAddress.HouseNumber;

            var Request = UserTestUtil.CreateMockAddressDto();
            Request.HouseNumber = "";

            _userRepository
                .Setup(x => x.GetAddressByIdAsync(UserAddressId, userId))
                .ReturnsAsync(UserAddress);

            _userService.UpdateUserAddressAsync(userId, UserAddressId, Request);

            Assert.That(UserAddress.HouseNumber, Is.EqualTo(original));
        }

        [Test]
        public async Task UpdateUserAddress_EmptyStreet_DoesNotUpdate()
        {
            var userId = Guid.NewGuid();

            var UserAddress = UserTestUtil.CreateMockAddress();

            var UserAddressId = UserAddress.UserAddressId;

            var original = UserAddress.Street;

            var Request = UserTestUtil.CreateMockAddressDto();

            Request.Street = "";

            _userRepository
                .Setup(x => x.GetAddressByIdAsync(UserAddressId, userId))
                .ReturnsAsync(UserAddress);

            _userService.UpdateUserAddressAsync(userId, UserAddressId, Request);

            Assert.That(UserAddress.Street, Is.EqualTo(original));
        }

        [Test]
        public async Task UpdateUserAddress_EmptyLandmark_DoesNotUpdate()
        {
            var userId = Guid.NewGuid();

            var UserAddress = UserTestUtil.CreateMockAddress();

            var UserAddressId = UserAddress.UserAddressId;

            var original = UserAddress.Landmark;

            var Request = UserTestUtil.CreateMockAddressDto();

            Request.Landmark = "";

            _userRepository
                .Setup(x => x.GetAddressByIdAsync(UserAddressId, userId))
                .ReturnsAsync(UserAddress);

            _userService.UpdateUserAddressAsync(userId, UserAddressId, Request);

            Assert.That(UserAddress.Landmark, Is.EqualTo(original));
        }

        [Test]
        public async Task UpdateUserAddress_EmptyCity_DoesNotUpdate()
        {
            var userId = Guid.NewGuid();

            var UserAddress = UserTestUtil.CreateMockAddress();

            var UserAddressId = UserAddress.UserAddressId;

            var original = UserAddress.City;

            var Request = UserTestUtil.CreateMockAddressDto();

            Request.City = "";

            _userRepository
                .Setup(x => x.GetAddressByIdAsync(UserAddressId, userId))
                .ReturnsAsync(UserAddress);

            _userService.UpdateUserAddressAsync(userId, UserAddressId, Request);

            Assert.That(UserAddress.City, Is.EqualTo(original));
        }

        [Test]
        public async Task UpdateUserAddress_EmptyState_DoesNotUpdate()
        {
            var userId = Guid.NewGuid();

            var UserAddress = UserTestUtil.CreateMockAddress();

            var UserAddressId = UserAddress.UserAddressId;

            var original = UserAddress.State;

            var Request = UserTestUtil.CreateMockAddressDto();

            Request.State = "";

            _userRepository
                .Setup(x => x.GetAddressByIdAsync(UserAddressId, userId))
                .ReturnsAsync(UserAddress);

            _userService.UpdateUserAddressAsync(userId, UserAddressId, Request);

            Assert.That(UserAddress.State, Is.EqualTo(original));
        }

        [Test]
        public async Task UpdateUserAddress_EmptyPincode_DoesNotUpdate()
        {
            var userId = Guid.NewGuid();

            var UserAddress = UserTestUtil.CreateMockAddress();

            var UserAddressId = UserAddress.UserAddressId;

            var original = UserAddress.Pincode;

            var Request = UserTestUtil.CreateMockAddressDto();

            Request.Pincode = "";

            _userRepository
                .Setup(x => x.GetAddressByIdAsync(UserAddressId, userId))
                .ReturnsAsync(UserAddress);

            _userService.UpdateUserAddressAsync(userId, UserAddressId, Request);

            Assert.That(UserAddress.Pincode, Is.EqualTo(original));
        }

        [Test]
        public async Task UpdateUserAddress_ValidRequest_SavesChanges()
        {
            var userId = Guid.NewGuid();

            var UserAddress = UserTestUtil.CreateMockAddress();

            var UserAddressId = UserAddress.UserAddressId;

            var Request = UserTestUtil.CreateMockAddressDto();

            _userRepository
                .Setup(x => x.GetAddressByIdAsync(UserAddressId, userId))
                .ReturnsAsync(UserAddress);

            await _userService.UpdateUserAddressAsync(userId, UserAddressId, Request);

            _appDbContext.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }
    }
}
