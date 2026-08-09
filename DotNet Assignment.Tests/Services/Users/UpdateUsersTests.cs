

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
    public class UpdateUsersTests
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
        public async Task UpdateUser_InvalidId_ThrowsException()
        {
            var requestDto = UserTestUtil.CreateMockUpdateUserDto();

            var userId = Guid.NewGuid();

            _userRepository
                .Setup(x=> x.GetUserByIdAsync(userId))
                .ReturnsAsync((User)null);

            Func<Task> action =() => _userService.UpdateUserAsync(userId, requestDto);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo("User not found"));
        }

        [Test]
        public async Task UpdateUser_ValidName_UpdatesName()
        {
            var user = UserTestUtil.CreateMockUser();

            var userId = user.UserId;

            var requestDto = UserTestUtil.CreateMockUpdateUserDto();

            requestDto.PhoneNumber = null;

            _userRepository
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            await _userService.UpdateUserAsync(userId, requestDto)  ;

            Assert.That(user.Name, Is.EqualTo(requestDto.Name));
        }

        [Test]
        public async Task UpdateUser_ValidPhoneNumber_UpdatesPhoneNumber()
        {
            var user = UserTestUtil.CreateMockUser();

            var userId = user.UserId;

            var requestDto = UserTestUtil.CreateMockUpdateUserDto();

            requestDto.Name = null;

            _userRepository
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            await _userService.UpdateUserAsync(userId, requestDto);

            Assert.That(user.PhoneNumber, Is.EqualTo(requestDto.PhoneNumber));
        }

        [Test]
        public async Task UpdateUser_ValidNameAndPhoneNumber_UpdatesBothFields()
        {
            var user = UserTestUtil.CreateMockUser();

            var userId = user.UserId;

            var requestDto = UserTestUtil.CreateMockUpdateUserDto();

            _userRepository
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            await _userService.UpdateUserAsync(userId, requestDto);

            Assert.That(user.Name, Is.EqualTo(requestDto.Name));

            Assert.That(user.PhoneNumber, Is.EqualTo(requestDto.PhoneNumber));
        }

        [Test]
        public async Task UpdateUser_NullName_DoesNotUpdateName()
        {
            var user = UserTestUtil.CreateMockUser();

            var userId = user.UserId;

            var originalName = user.Name;

            var requestDto = UserTestUtil.CreateMockUpdateUserDto();

            requestDto.Name = "";

            _userRepository
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            await _userService.UpdateUserAsync(userId, requestDto);

            Assert.That(user.Name, Is.EqualTo(originalName));
        }

        [Test]
        public async Task UpdateUser_NullPhoneNumber_DoesNotUpdatePhoneNumber()
        {
            var user = UserTestUtil.CreateMockUser();

            var userId = user.UserId;

            var originalPhone = user.PhoneNumber;

            var requestDto = UserTestUtil.CreateMockUpdateUserDto();
            requestDto.PhoneNumber = "";

            _userRepository
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            await _userService.UpdateUserAsync(userId, requestDto);

            Assert.That(user.PhoneNumber, Is.EqualTo(originalPhone));
        }

        [Test]
        public async Task UpdateUser_ValidRequest_SavesChanges()
        {
            var user = UserTestUtil.CreateMockUser();

            var userId = user.UserId;

            var requestDto = UserTestUtil.CreateMockUpdateUserDto();

            _userRepository
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            await _userService.UpdateUserAsync(userId, requestDto);

            _appDbContext.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }
    }
}
