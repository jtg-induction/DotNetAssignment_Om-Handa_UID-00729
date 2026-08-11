using DotNet_Assignment.Constants;
using DotNet_Assignment.Data;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Repository.RefreshTokens;
using DotNet_Assignment.Repository.Users;
using DotNet_Assignment.Services.Users;
using DotNet_Assignment.Tests.Constants;
using DotNet_Assignment.Tests.Utils;
using DotNet_Assignment.Utils;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet_Assignment.Tests.Services.Users
{
    [TestFixture]
    public class ChangePasswordTests
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
        /// ChangePassword Function - User Not found - Throws exception
        /// </summary>
        [Test]
        public async Task ChangePassword_UserNotFound_ThrowsException()
        {
            var userId = Guid.NewGuid();

            var request = new ChangePasswordDto
            {
                OldPassword = MockConstants.MockPassword,
                NewPassword = MockConstants.MockNewPassword
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync((User)null);

            Func<Task> Action = async () =>
                await _userService.ChangePasswordAsync(userId, request);

            var exception = Assert.CatchAsync<Exception>(Action);

            Assert.That(exception.Message,Is.EqualTo(ExceptionMessages.UserNotFound));

            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        /// <summary>
        /// ChangePassword Function - User Deactivated - Throws exception
        /// </summary>
        [Test]
        public async Task ChangePassword_UserDeactivated_ThrowsException()
        {
            var userId = Guid.NewGuid();

            var request = new ChangePasswordDto
            {
                OldPassword = MockConstants.MockPassword,
                NewPassword = MockConstants.MockNewPassword
            };

            var user = new User
            {
                UserId = userId,
                Password = Hasher.Hash(MockConstants.MockPassword),
                IsDeleted = true
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            Func<Task> Action = async () =>
                await _userService.ChangePasswordAsync(userId, request);

            var exception = Assert.CatchAsync<Exception>(Action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.UserDeactivated));

            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        /// <summary>
        /// ChangePassword Function - Incorrect old password - Throws exception
        /// </summary>
        [Test]
        public async Task ChangePassword_IncorrectOldPassword_ThrowsException()
        {
            var userId = Guid.NewGuid();

            var request = new ChangePasswordDto
            {
                OldPassword = MockConstants.MockWrongPassword,
                NewPassword = MockConstants.MockNewPassword
            };

            var user = new User
            {
                UserId = userId,
                Password = Hasher.Hash(MockConstants.MockPassword),
                IsDeleted = false
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            Func<Task> Action = async () =>
                await _userService.ChangePasswordAsync(userId, request);

            var exception = Assert.CatchAsync<Exception>(Action);

            Assert.That(exception.Message,Is.EqualTo(ExceptionMessages.OldPasswordIncorrect));

            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        /// <summary>
        /// ChangePassword Function - Valid Request - Changes Password
        /// </summary>
        [Test]
        public async Task ChangePassword_ValidRequest_ChangesPassword()
        {
            var userId = Guid.NewGuid();

            var request = new ChangePasswordDto
            {
                OldPassword = MockConstants.MockPassword,
                NewPassword = MockConstants.MockNewPassword
            };

            var user = new User
            {
                UserId = userId,
                Password = Hasher.Hash(MockConstants.MockPassword),
                IsDeleted = false
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            Func<Task> Action = async () => await _userService.ChangePasswordAsync(userId, request);

            Assert.DoesNotThrowAsync(Action);

            Assert.That(Hasher.Verify(request.NewPassword,user.Password),Is.True);

            _appDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// ChangePassword Function - Valid Request - Saves Password
        /// </summary>
        [Test]
        public async Task ChangePassword_ValidRequest_SavesChanges()
        {
            var userId = Guid.NewGuid();

            var request = new ChangePasswordDto
            {
                OldPassword = MockConstants.MockPassword,
                NewPassword = MockConstants.MockNewPassword
            };

            var user = new User
            {
                UserId = userId,
                Password = Hasher.Hash(MockConstants.MockPassword),
                IsDeleted = false
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            Func<Task> Action = async () => await _userService.ChangePasswordAsync(userId, request);

            Assert.DoesNotThrowAsync(Action);

            _appDbContext.Verify(x => x.SaveChangesAsync(),Times.Once);
        }

    }
}
