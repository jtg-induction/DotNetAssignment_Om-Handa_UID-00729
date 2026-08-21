using DotNet_Assignment.Constants;
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

        /// <summary>
        /// DeactivateUser Function - Invalid User Id - throws exception
        /// </summary>
        [Test]
        public void DeactivateUser_InvalidId_ThrowsException()
        {
            var userId = Guid.NewGuid();

            _userRepository
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync((User)null);

            Func<Task> Action = async() => await _userService.DeactivateUserAsync(userId);

            var exception = Assert.CatchAsync<Exception>(Action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.UserNotFound));

            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        /// <summary>
        /// DeactivateUser Function - Valid User Id - user marked deleted
        /// </summary>
        [Test]
        public void DeactivateUser_ValidUser_MarksUserDeleted()
        {
            var userId = Guid.NewGuid();

            var user = UserTestUtil.CreateMockUser();

            _userRepository
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            Func<Task> Action = async () =>await _userService.DeactivateUserAsync(userId);

            Assert.DoesNotThrowAsync(Action);

            Assert.That(user.IsDeleted, Is.True);
            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// DeactivateUser Function - Valid User Id - deletes all refresh tokens
        /// </summary>
        [Test]
        public void DeactivateUser_DeletesRefreshTokens()
        {
            var userId = Guid.NewGuid();

            var user = UserTestUtil.CreateMockUser();

            _userRepository
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            Func<Task> Action = async () => await _userService.DeactivateUserAsync(userId);

            Assert.DoesNotThrowAsync(Action);

            _refreshTokenRepository.Verify(
                x => x.DeleteTokensByUserId(userId),
                Times.Once);

            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// DeactivateUser Function - Valid User Id - saves changes
        /// </summary>
        [Test]
        public void DeactivateUser_SavesChanges()
        {
            var userId = Guid.NewGuid();

            var user = UserTestUtil.CreateMockUser();

            _userRepository
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            Func<Task> Action = async () => await _userService.DeactivateUserAsync(userId);

            Assert.DoesNotThrowAsync(Action);

            _appDbContext.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);

            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
    }
}
