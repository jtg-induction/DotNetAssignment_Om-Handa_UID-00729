using DotNet_Assignment.Data;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Repository.RefreshTokens;
using DotNet_Assignment.Repository.Users;
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
        public async Task AddUserAddress_UserNotFound_ThrowsException()
        {
            var userId = Guid.NewGuid();

            var Request = UserTestUtil.CreateMockAddressDto();

            _userRepository
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync((User)null);

            Func<Task> action = async () => await _userService.AddUserAddressAsync(userId, Request);

            var Exception = Assert.CatchAsync<Exception>(action);

            Assert.That(Exception.Message, Is.EqualTo("User not found"));
        }

        [Test]
        public async Task AddUserAddress_UserDeactivated_ThrowsException()
        {
            var User = UserTestUtil.CreateMockUser();

            User.IsDeleted = true;

            var Request = UserTestUtil.CreateMockAddressDto();

            _userRepository
                .Setup(x => x.GetUserByIdAsync(User.UserId))
                .ReturnsAsync(User);

            Func<Task> action = async () => await _userService.AddUserAddressAsync(User.UserId, Request);

            var Exception = Assert.CatchAsync<Exception>(action);

            Assert.That(Exception.Message, Is.EqualTo("User Already Deactivated"));
        }

        [Test]
        public async Task AddUserAddress_ValidRequest_AddsAddress()
        {
            var User = UserTestUtil.CreateMockUser();

            var Request = UserTestUtil.CreateMockAddressDto();

            _userRepository
                .Setup(x => x.GetUserByIdAsync(User.UserId))
                .ReturnsAsync(User);

            await _userService.AddUserAddressAsync(User.UserId, Request);

            _userRepository.Verify(
                x => x.AddAddress(It.IsAny<UserAddress>()),
                Times.Once);
        }

        [Test]
        public async Task AddUserAddress_ValidRequest_SavesChanges()
        {
            var User = UserTestUtil.CreateMockUser();

            var Request = UserTestUtil.CreateMockAddressDto();

            _userRepository
                .Setup(x => x.GetUserByIdAsync(User.UserId))
                .ReturnsAsync(User);

            await _userService.AddUserAddressAsync(User.UserId, Request);

            _appDbContext.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }
    }
}
