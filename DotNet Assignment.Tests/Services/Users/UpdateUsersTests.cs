

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
            var RequestDto = UserTestUtil.CreateMockUpdateUserDto();

            var UserId = Guid.NewGuid();

            _userRepository
                .Setup(x=> x.GetUserByIdAsync(UserId))
                .ReturnsAsync((User)null);

            Func<Task> action =() => _userService.UpdateUserAsync(UserId, RequestDto);

            var Exception = Assert.CatchAsync<Exception>(action);

            Assert.That(Exception.Message, Is.EqualTo("User not found"));
        }

        [Test]
        public async Task UpdateUser_ValidName_UpdatesName()
        {
            var User = UserTestUtil.CreateMockUser();

            var UserId = User.UserId;

            var RequestDto = UserTestUtil.CreateMockUpdateUserDto();

            RequestDto.PhoneNumber = null;

            _userRepository
                .Setup(x => x.GetUserByIdAsync(UserId))
                .ReturnsAsync(User);

            await _userService.UpdateUserAsync(UserId, RequestDto)  ;

            Assert.That(User.Name, Is.EqualTo(RequestDto.Name));
        }

        [Test]
        public async Task UpdateUser_ValidPhoneNumber_UpdatesPhoneNumber()
        {
            var User = UserTestUtil.CreateMockUser();

            var UserId = User.UserId;

            var RequestDto = UserTestUtil.CreateMockUpdateUserDto();

            RequestDto.Name = null;

            _userRepository
                .Setup(x => x.GetUserByIdAsync(UserId))
                .ReturnsAsync(User);

            await _userService.UpdateUserAsync(UserId, RequestDto);

            Assert.That(User.PhoneNumber, Is.EqualTo(RequestDto.PhoneNumber));
        }

        [Test]
        public async Task UpdateUser_ValidNameAndPhoneNumber_UpdatesBothFields()
        {
            var User = UserTestUtil.CreateMockUser();

            var UserId = User.UserId;

            var RequestDto = UserTestUtil.CreateMockUpdateUserDto();

            _userRepository
                .Setup(x => x.GetUserByIdAsync(UserId))
                .ReturnsAsync(User);

            await _userService.UpdateUserAsync(UserId, RequestDto);

            Assert.That(User.Name, Is.EqualTo(RequestDto.Name));

            Assert.That(User.PhoneNumber, Is.EqualTo(RequestDto.PhoneNumber));
        }

        [Test]
        public async Task UpdateUser_NullName_DoesNotUpdateName()
        {
            var User = UserTestUtil.CreateMockUser();

            var UserId = User.UserId;

            var originalName = User.Name;

            var RequestDto = UserTestUtil.CreateMockUpdateUserDto();

            RequestDto.Name = "";

            _userRepository
                .Setup(x => x.GetUserByIdAsync(UserId))
                .ReturnsAsync(User);

            await _userService.UpdateUserAsync(UserId, RequestDto);

            Assert.That(User.Name, Is.EqualTo(originalName));
        }

        [Test]
        public async Task UpdateUser_NullPhoneNumber_DoesNotUpdatePhoneNumber()
        {
            var User = UserTestUtil.CreateMockUser();

            var UserId = User.UserId;

            var originalPhone = User.PhoneNumber;

            var RequestDto = UserTestUtil.CreateMockUpdateUserDto();
            RequestDto.PhoneNumber = "";

            _userRepository
                .Setup(x => x.GetUserByIdAsync(UserId))
                .ReturnsAsync(User);

            await _userService.UpdateUserAsync(UserId, RequestDto);

            Assert.That(User.PhoneNumber, Is.EqualTo(originalPhone));
        }

        [Test]
        public async Task UpdateUser_ValidRequest_SavesChanges()
        {
            var User = UserTestUtil.CreateMockUser();

            var UserId = User.UserId;

            var RequestDto = UserTestUtil.CreateMockUpdateUserDto();

            _userRepository
                .Setup(x => x.GetUserByIdAsync(UserId))
                .ReturnsAsync(User);

            await _userService.UpdateUserAsync(UserId, RequestDto);

            _appDbContext.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }
    }
}
