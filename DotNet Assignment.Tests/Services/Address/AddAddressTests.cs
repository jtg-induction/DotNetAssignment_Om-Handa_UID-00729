using DotNet_Assignment.Constants;
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace DotNet_Assignment.Tests.Services.Users
{
    public class AddAddressTests
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

        /// <summary>
        ///  AddUserAddress Function - User not found - throws Exception
        /// </summary>
        [Test]
        public async Task AddUserAddress_UserNotFound_ThrowsException()
        {
            var userId = Guid.NewGuid();

            var request = UserTestUtil.CreateMockAddressDto();

            _userRepository
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync((User)null);

            Func<Task> action = async () => await _addressService.AddUserAddressAsync(userId, request);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.UserNotFound));
        }

        /// <summary>
        ///  AddUserAddress Function - User Deactivated - throws Exception
        /// </summary>
        [Test]
        public async Task AddUserAddress_UserDeactivated_ThrowsException()
        {
            var user = UserTestUtil.CreateMockUser();

            user.IsDeleted = true;

            var request = UserTestUtil.CreateMockAddressDto();

            _userRepository
                .Setup(x => x.GetUserByIdAsync(user.UserId))
                .ReturnsAsync(user);

            Func<Task> action = async () => await _addressService.AddUserAddressAsync(user.UserId, request);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.UserDeactivated));

            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        /// <summary>
        ///  AddUserAddress Function - Valid Request - Adds Address
        /// </summary>
        [Test]
        public async Task AddUserAddress_ValidRequest_AddsAddress()
        {
            var user = UserTestUtil.CreateMockUser();

            var request = UserTestUtil.CreateMockAddressDto();

            _userRepository
                .Setup(x => x.GetUserByIdAsync(user.UserId))
                .ReturnsAsync(user);

            Func<Task> action = async () => await _addressService.AddUserAddressAsync(user.UserId, request);

            Assert.DoesNotThrowAsync(action);

            _addressRepository.Verify(
                x => x.AddAddress(It.IsAny<UserAddress>()),
                Times.Once);

            _appDbContext.Verify(x => x.SaveChangesAsync(),Times.Once);
        }

        /// <summary>
        ///  AddUserAddress Function - Valid Request - Saves Address
        /// </summary>
        [Test]
        public async Task AddUserAddress_ValidRequest_SavesChanges()
        {
            var user = UserTestUtil.CreateMockUser();

            var request = UserTestUtil.CreateMockAddressDto();

            _userRepository
                .Setup(x => x.GetUserByIdAsync(user.UserId))
                .ReturnsAsync(user);

            Func<Task> action = async () => await _addressService.AddUserAddressAsync(user.UserId, request);

            Assert.DoesNotThrowAsync(action);

            _appDbContext.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }
    }
}
