using DotNet_Assignment.Repository.RefreshTokens;
using DotNet_Assignment.Repository.Users;
using DotNet_Assignment.Services.Users;
using DotNet_Assignment.Tests.Utils;
using DotNet_Assignment.Models.Entities;
using Moq;
using NUnit.Framework;
using System;
using DotNet_Assignment.Data;
using System.Threading.Tasks;

namespace DotNet_Assignment.Tests.Services.Users
{
    [TestFixture]
    public class DeactivateUserTests
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
        public void DeactivateUser_InvalidId_ThrowsException()
        {
            var RequestDto = UserTestUtil.CreateMockUpdateUserDto();

            var UserId = Guid.NewGuid();

            _userRepository
                .Setup(x => x.GetUserByIdAsync(UserId))
                .ReturnsAsync((User)null);

            Func<Task> action = () => _userService.UpdateUserAsync(UserId, RequestDto);

            var Exception = Assert.CatchAsync<Exception>(action);

            Assert.That(Exception.Message, Is.EqualTo("User not found"));
        }

        [Test]
        public void DeactivateUser_ValidUser_MarksUserDeleted()
        {
            var userId = Guid.NewGuid();

            var user = UserTestUtil.CreateMockUser();

            _userRepository
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            _userService.DeactivateUserAsync(userId);

            Assert.That(user.IsDeleted, Is.True);
        }

        [Test]
        public void DeactivateUser_DeletesRefreshTokens()
        {
            var userId = Guid.NewGuid();

            var user = UserTestUtil.CreateMockUser();

            _userRepository
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            _userService.DeactivateUserAsync(userId);

            _refreshTokenRepository.Verify(
                x => x.DeleteTokensByUserId(userId),
                Times.Once);
        }

        [Test]
        public void DeactivateUser_SavesChanges()
        {
            var userId = Guid.NewGuid();

            var user = UserTestUtil.CreateMockUser();

            _userRepository
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            _userService.DeactivateUserAsync(userId);

            _appDbContext.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }
    }
}
