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
    public class DeactivateUserTests
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
        public void DeactivateUser_InvalidId_ThrowsException()
        {
            var RequestDto = UserTestUtil.CreateMockUpdateUserDto();

            var UserId = Guid.NewGuid();

            _userRepository
                .Setup(x => x.GetUserById(UserId))
                .Returns((User)null);

            Action action = () => _userService.UpdateUser(UserId, RequestDto);

            var Exception = Assert.Throws<Exception>(action);

            Assert.That(Exception.Message, Is.EqualTo("User not found"));
        }

        [Test]
        public void DeactivateUser_ValidUser_MarksUserDeleted()
        {
            var userId = Guid.NewGuid();

            var user = UserTestUtil.CreateMockUser();

            _userRepository
                .Setup(x => x.GetUserById(userId))
                .Returns(user);

            _userService.DeactivateUser(userId);

            Assert.That(user.IsDeleted, Is.True);
        }

        [Test]
        public void DeactivateUser_DeletesRefreshTokens()
        {
            var userId = Guid.NewGuid();

            var user = UserTestUtil.CreateMockUser();

            _userRepository
                .Setup(x => x.GetUserById(userId))
                .Returns(user);

            _userService.DeactivateUser(userId);

            _refreshTokenRepository.Verify(
                x => x.DeleteTokensByUserId(userId),
                Times.Once);
        }

        [Test]
        public void DeactivateUser_SavesUserChanges()
        {
            var userId = Guid.NewGuid();

            var user = UserTestUtil.CreateMockUser();

            _userRepository
                .Setup(x => x.GetUserById(userId))
                .Returns(user);

            _userService.DeactivateUser(userId);

            _userRepository.Verify(
                x => x.Save(),
                Times.Once);
        }

        [Test]
        public void DeactivateUser_SavesRefreshTokenChanges()
        {
            var userId = Guid.NewGuid();

            var user = UserTestUtil.CreateMockUser();

            _userRepository
                .Setup(x => x.GetUserById(userId))
                .Returns(user);

            _userService.DeactivateUser(userId);

            _refreshTokenRepository.Verify(
                x => x.Save(),
                Times.Once);
        }

    }
}
