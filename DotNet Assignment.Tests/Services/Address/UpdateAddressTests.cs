
using DotNet_Assignment.Data;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Repository.Address;
using DotNet_Assignment.Repository.RefreshTokens;
using DotNet_Assignment.Repository.Users;
using DotNet_Assignment.Services.Address;
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
        private Mock<IAddressRepository> _addressRepository;
        private Mock<IUserRepository> _userRepository;
        private Mock<AppDbContext> _appDbContext;
        private AddressService _addressService;

        [SetUp]
        public void Setup()
        {
            _addressRepository = new Mock<IAddressRepository>();
            _userRepository = new Mock<IUserRepository>();
            _appDbContext = new Mock<AppDbContext>();

            _addressService = new AddressService(
                _addressRepository.Object,
                _userRepository.Object,
                _appDbContext.Object
            );
        }

        [Test]
        public async Task UpdateAddress_InvalidId_ThrowsException()
        {
            var requestDto = UserTestUtil.CreateMockAddressDto();

            var userAddressId = Guid.NewGuid();

            var userId = Guid.NewGuid();

            _addressRepository
                .Setup(x => x.GetAddressByIdAsync(userAddressId, userId))
                .ReturnsAsync((UserAddress)null);

            Func<Task> action = () => _addressService.UpdateUserAddressAsync(userId,userAddressId, requestDto);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo("Address not found"));
        }

        [Test]
        public async Task UpdateUserAddress_ValidRequest_UpdatesAllFields()
        {
            var userId = Guid.NewGuid();

            var userAddress = UserTestUtil.CreateMockAddress();

            var userAddressId = userAddress.UserAddressId;

            var request = UserTestUtil.CreateMockAddressDto();

            _addressRepository
                .Setup(x => x.GetAddressByIdAsync(userAddressId, userId))
                .ReturnsAsync(userAddress);

            await _addressService.UpdateUserAddressAsync(userId, userAddressId, request);

            Assert.That(userAddress.HouseNumber, Is.EqualTo(request.HouseNumber));
            Assert.That(userAddress.Street, Is.EqualTo(request.Street));
            Assert.That(userAddress.Landmark, Is.EqualTo(request.Landmark));
            Assert.That(userAddress.City, Is.EqualTo(request.City));
            Assert.That(userAddress.State, Is.EqualTo(request.State));
            Assert.That(userAddress.Pincode, Is.EqualTo(request.Pincode));
        }

        [Test]
        public async Task UpdateUserAddress_EmptyHouseNumber_DoesNotUpdate()
        {
            var userId = Guid.NewGuid();

            var userAddress = UserTestUtil.CreateMockAddress();

            var userAddressId = userAddress.UserAddressId;

            var original = userAddress.HouseNumber;

            var request = UserTestUtil.CreateMockAddressDto();
            request.HouseNumber = "";

            _addressRepository
                .Setup(x => x.GetAddressByIdAsync(userAddressId, userId))
                .ReturnsAsync(userAddress);

            await _addressService.UpdateUserAddressAsync(userId, userAddressId, request);

            Assert.That(userAddress.HouseNumber, Is.EqualTo(original));
        }

        [Test]
        public async Task UpdateUserAddress_EmptyStreet_DoesNotUpdate()
        {
            var userId = Guid.NewGuid();

            var userAddress = UserTestUtil.CreateMockAddress();

            var userAddressId = userAddress.UserAddressId;

            var original = userAddress.Street;

            var request = UserTestUtil.CreateMockAddressDto();

            request.Street = "";

            _addressRepository
                .Setup(x => x.GetAddressByIdAsync(userAddressId, userId))
                .ReturnsAsync(userAddress);

            await _addressService.UpdateUserAddressAsync(userId, userAddressId, request);

            Assert.That(userAddress.Street, Is.EqualTo(original));
        }

        [Test]
        public async Task UpdateUserAddress_EmptyLandmark_DoesNotUpdate()
        {
            var userId = Guid.NewGuid();

            var userAddress = UserTestUtil.CreateMockAddress();

            var userAddressId = userAddress.UserAddressId;

            var original = userAddress.Landmark;

            var request = UserTestUtil.CreateMockAddressDto();

            request.Landmark = "";

            _addressRepository
                .Setup(x => x.GetAddressByIdAsync(userAddressId, userId))
                .ReturnsAsync(userAddress);

            await _addressService.UpdateUserAddressAsync(userId, userAddressId, request);

            Assert.That(userAddress.Landmark, Is.EqualTo(original));
        }

        [Test]
        public async Task UpdateUserAddress_EmptyCity_DoesNotUpdate()
        {
            var userId = Guid.NewGuid();

            var userAddress = UserTestUtil.CreateMockAddress();

            var userAddressId = userAddress.UserAddressId;

            var original = userAddress.City;

            var request = UserTestUtil.CreateMockAddressDto();

            request.City = "";

            _addressRepository
                .Setup(x => x.GetAddressByIdAsync(userAddressId, userId))
                .ReturnsAsync(userAddress);

            await _addressService.UpdateUserAddressAsync(userId, userAddressId, request);

            Assert.That(userAddress.City, Is.EqualTo(original));
        }

        [Test]
        public async Task UpdateUserAddress_EmptyState_DoesNotUpdate()
        {
            var userId = Guid.NewGuid();

            var userAddress = UserTestUtil.CreateMockAddress();

            var userAddressId = userAddress.UserAddressId;

            var original = userAddress.State;

            var request = UserTestUtil.CreateMockAddressDto();

            request.State = "";

            _addressRepository
                .Setup(x => x.GetAddressByIdAsync(userAddressId, userId))
                .ReturnsAsync(userAddress);

            await _addressService.UpdateUserAddressAsync(userId, userAddressId, request);

            Assert.That(userAddress.State, Is.EqualTo(original));
        }

        [Test]
        public async Task UpdateUserAddress_EmptyPincode_DoesNotUpdate()
        {
            var userId = Guid.NewGuid();

            var userAddress = UserTestUtil.CreateMockAddress();

            var userAddressId = userAddress.UserAddressId;

            var original = userAddress.Pincode;

            var request = UserTestUtil.CreateMockAddressDto();

            request.Pincode = "";

            _addressRepository
                .Setup(x => x.GetAddressByIdAsync(userAddressId, userId))
                .ReturnsAsync(userAddress);

            await _addressService.UpdateUserAddressAsync(userId, userAddressId, request);

            Assert.That(userAddress.Pincode, Is.EqualTo(original));
        }

        [Test]
        public async Task UpdateUserAddress_ValidRequest_SavesChanges()
        {
            var userId = Guid.NewGuid();

            var userAddress = UserTestUtil.CreateMockAddress();

            var userAddressId = userAddress.UserAddressId;

            var request = UserTestUtil.CreateMockAddressDto();

            _addressRepository
                .Setup(x => x.GetAddressByIdAsync(userAddressId, userId))
                .ReturnsAsync(userAddress);

            await _addressService.UpdateUserAddressAsync(userId, userAddressId, request);

            _appDbContext.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }
    }
}
