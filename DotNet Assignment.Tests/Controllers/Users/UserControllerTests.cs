using DotNet_Assignment.Constants;
using DotNet_Assignment.Controllers;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Services.Auth;
using DotNet_Assignment.Services.Users;
using DotNet_Assignment.Tests.Utils;
using Moq;
using NUnit.Framework;
using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Results;

namespace DotNet_Assignment.Tests.Controllers.Users
{
    [TestFixture]
    public class UserControllerTests
    {
        private Mock<IUserService> _userService;
        private UserController _userController;

        [SetUp]
        public void Setup()
        {
            _userService = new Mock<IUserService>();

            _userController = new UserController(_userService.Object);
        }

        /// <summary>
        /// UpdateUser 
        /// - Sent valid request - updates user and returns success
        /// </summary>
        [Test]
        public async Task UpdateUser_ValidRequest_ReturnsSuccess()
        {
            var userId = Guid.NewGuid();

            SetUser(userId);

            var dto = UserTestUtil.CreateMockUpdateUserDto();

            var result = await _userController.UpdateUserAsync(dto);

            var okResult = result as OkNegotiatedContentResult<ApiResponseDto<object>>;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Content.IsSuccess, Is.True);
            Assert.That(okResult.Content.Message, Is.EqualTo(SuccessMessages.ProfileUpdated));

            _userService.Verify(x => x.UpdateUserAsync(userId, dto), Times.Once);
        }

        /// <summary>
        /// UpdateUser action - Service throws exception - returns exception
        /// </summary>
        [Test]
        public async Task UpdateUser_ServiceThrows_ReturnsFailure()
        {
            var userId = Guid.NewGuid();

            SetUser(userId);

            var dto = UserTestUtil.CreateMockUpdateUserDto();

            _userService.Setup(x => x.UpdateUserAsync(userId, dto))
                        .ThrowsAsync(new Exception(ExceptionMessages.UserNotFound));

            Func<Task> action = async () => await _userController.UpdateUserAsync(dto);

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.UserNotFound));
        }

        /// <summary>
        /// DeactivateUser action - Sent valid request - Deactivates user and returns success
        /// </summary>
        [Test]
        public async Task DeactivateUser_ValidRequest_ReturnsSuccess()
        {
            var userId = Guid.NewGuid();

            SetUser(userId);

            var result = await _userController.DeactivateUserAsync();

            var okResult = result as OkNegotiatedContentResult<ApiResponseDto<object>>;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Content.IsSuccess, Is.True);
            Assert.That(okResult.Content.Message, Is.EqualTo(SuccessMessages.UserDeactivated));

            _userService.Verify(x => x.DeactivateUserAsync(userId), Times.Once);
        }

        /// <summary>
        /// DeactivateUser action - Service throws exception - returns exception
        /// </summary>
        [Test]
        public async Task DeactivateUser_ServiceThrows_ReturnsFailure()
        {
            var userId = Guid.NewGuid();

            SetUser(userId);

            _userService.Setup(x => x.DeactivateUserAsync(userId))
                        .ThrowsAsync(new Exception(ExceptionMessages.UserNotFound));

            Func<Task> action = async()=> await _userController.DeactivateUserAsync();

            var exception = Assert.CatchAsync<Exception>(action);

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.UserNotFound));
        }

        /// <summary>
        /// Mocks User Identity to send with Context
        /// </summary>
        /// <param name="userId"></param>
        private void SetUser(Guid userId)
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            });

            _userController.ControllerContext = new HttpControllerContext();
            _userController.User = new ClaimsPrincipal(identity);
        }
    }
}
