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
            Assert.That(okResult.Content.Message, Is.EqualTo("Profile updated"));

            _userService.Verify(x => x.UpdateUserAsync(userId, dto), Times.Once);
        }

        [Test]
        public async Task UpdateUser_ServiceThrows_ReturnsFailure()
        {
            var userId = Guid.NewGuid();

            SetUser(userId);

            var dto = UserTestUtil.CreateMockUpdateUserDto();

            _userService.Setup(x => x.UpdateUserAsync(userId, dto))
                        .ThrowsAsync(new Exception("User Not Found"));

            Func<Task> Action = async () => await _userController.UpdateUserAsync(dto);

            var exception = Assert.CatchAsync<Exception>(Action);

            Assert.That(exception.Message, Is.EqualTo("User Not Found"));
        }

        [Test]
        public async Task DeactivateUser_ValidRequest_ReturnsSuccess()
        {
            var userId = Guid.NewGuid();

            SetUser(userId);

            var result = await _userController.DeactivateUserAsync();

            var okResult = result as OkNegotiatedContentResult<ApiResponseDto<object>>;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Content.IsSuccess, Is.True);
            Assert.That(okResult.Content.Message, Is.EqualTo("User deactivated"));

            _userService.Verify(x => x.DeactivateUserAsync(userId), Times.Once);
        }

        [Test]
        public async Task DeactivateUser_ServiceThrows_ReturnsFailure()
        {
            var userId = Guid.NewGuid();

            SetUser(userId);

            _userService.Setup(x => x.DeactivateUserAsync(userId))
                        .ThrowsAsync(new Exception("User Not Found"));

            Func<Task> Action = async()=> await _userController.DeactivateUserAsync();

            var exception = Assert.CatchAsync<Exception>(Action);

            Assert.That(exception.Message, Is.EqualTo("User Not Found"));
        }

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
