

using DotNet_Assignment.Repository.RefreshTokens;
using DotNet_Assignment.Repository.Users;
using DotNet_Assignment.Services.Users;
using DotNet_Assignment.Tests.Utils;
using DotNet_Assignment.Models.Entities;
using Moq;
using NUnit.Framework;
using System;

namespace DotNet_Assignment.Tests.Services.Users
{
    [TestFixture]
    public class UpdateUsersTests
    {
        private Mock<IUserRepository> _userRepository;
        private Mock<IRefreshTokenRepository> _refreshTokenRepository;
        private UserService _userService;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _refreshTokenRepository = new Mock<IRefreshTokenRepository>();

            _userService= new UserService(
                _userRepository.Object,
                _refreshTokenRepository.Object
            );
        }

        [Test]
        public void UpdateUser_InvalidId_ThrowsException()
        {
            var RequestDto = UserTestUtil.CreateMockUpdateUserDto();

            var UserId = Guid.NewGuid();

            _userRepository
                .Setup(x=> x.GetUserById(UserId))
                .Returns((User)null);

            Action action =() => _userService.UpdateUser(UserId, RequestDto);

            var Exception = Assert.Throws<Exception>(action);

            Assert.That(Exception.Message, Is.EqualTo("User not found"));
        }

        [Test]
        public void UpdateUser_ValidName_UpdatesName()
        {
            var User = UserTestUtil.CreateMockUser();

            var UserId = User.UserId;

            var RequestDto = UserTestUtil.CreateMockUpdateUserDto();

            RequestDto.PhoneNumber = null;

            _userRepository
                .Setup(x => x.GetUserById(UserId))
                .Returns(User);

            _userService.UpdateUser(UserId, RequestDto);

            Assert.That(User.Name, Is.EqualTo(RequestDto.Name));
        }

        [Test]
        public void UpdateUser_ValidPhoneNumber_UpdatesPhoneNumber()
        {
            var User = UserTestUtil.CreateMockUser();

            var UserId = User.UserId;

            var RequestDto = UserTestUtil.CreateMockUpdateUserDto();

            RequestDto.Name = null;

            _userRepository
                .Setup(x => x.GetUserById(UserId))
                .Returns(User);

            _userService.UpdateUser(UserId, RequestDto);

            Assert.That(User.PhoneNumber, Is.EqualTo(RequestDto.PhoneNumber));
        }

        [Test]
        public void UpdateUser_ValidNameAndPhoneNumber_UpdatesBothFields()
        {
            var User = UserTestUtil.CreateMockUser();

            var UserId = User.UserId;

            var RequestDto = UserTestUtil.CreateMockUpdateUserDto();

            _userRepository
                .Setup(x => x.GetUserById(UserId))
                .Returns(User);

            _userService.UpdateUser(UserId, RequestDto);

            Assert.That(User.Name, Is.EqualTo(RequestDto.Name));

            Assert.That(User.PhoneNumber, Is.EqualTo(RequestDto.PhoneNumber));
        }

        [Test]
        public void UpdateUser_NullName_DoesNotUpdateName()
        {
            var User = UserTestUtil.CreateMockUser();

            var UserId = User.UserId;

            var originalName = User.Name;

            var RequestDto = UserTestUtil.CreateMockUpdateUserDto();

            RequestDto.Name = "";

            _userRepository
                .Setup(x => x.GetUserById(UserId))
                .Returns(User);

            _userService.UpdateUser(UserId, RequestDto);

            Assert.That(User.Name, Is.EqualTo(originalName));
        }

        [Test]
        public void UpdateUser_NullPhoneNumber_DoesNotUpdatePhoneNumber()
        {
            var User = UserTestUtil.CreateMockUser();

            var UserId = User.UserId;

            var originalPhone = User.PhoneNumber;

            var RequestDto = UserTestUtil.CreateMockUpdateUserDto();
            RequestDto.PhoneNumber = "";

            _userRepository
                .Setup(x => x.GetUserById(UserId))
                .Returns(User);

            _userService.UpdateUser(UserId, RequestDto);

            Assert.That(User.PhoneNumber, Is.EqualTo(originalPhone));
        }

        [Test]
        public void UpdateUser_ValidRequest_SavesChanges()
        {
            var User = UserTestUtil.CreateMockUser();

            var UserId = User.UserId;

            var RequestDto = UserTestUtil.CreateMockUpdateUserDto();

            _userRepository
                .Setup(x => x.GetUserById(UserId))
                .Returns(User);

            _userService.UpdateUser(UserId, RequestDto);

            _userRepository.Verify(x => x.Save(), Times.Once);
        }
    }
}
