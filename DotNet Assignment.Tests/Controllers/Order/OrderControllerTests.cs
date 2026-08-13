using DotNet_Assignment.Constants;
using DotNet_Assignment.Controllers;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Services.Address;
using DotNet_Assignment.Services.Orders;
using DotNet_Assignment.Tests.Utils;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Results;

namespace DotNet_Assignment.Tests.Controllers.Order
{
    internal class OrderControllerTests
    {
        [TestFixture]
        public class AddressControllerTests
        {
            private Mock<IOrderService> _orderService;
            private OrderController _orderController;

            [SetUp]
            public void Setup()
            {
                _orderService = new Mock<IOrderService>();

                _orderController = new OrderController(_orderService.Object);
            }

            /// <summary>
            /// PlaceOrder Function - Valid Request - Returns Success
            /// </summary>
            [Test]
            public async Task PlaceOrder_ValidRequest_ReturnsSuccess()
            {
                var userId = Guid.NewGuid();
                var request = OrderTestUtil.CreateMockOrderRequestDto();
                var orderResponse = new OrderResponseDto();

                _orderService
                    .Setup(x => x.PlaceOrderAsync(userId, request))
                    .ReturnsAsync(orderResponse);

                SetUser(userId);

                var result = await _orderController.PlaceOrderAsync(request);

                var okResult =result as OkNegotiatedContentResult<ApiResponseDto<OrderResponseDto>>;

                Assert.That(okResult, Is.Not.Null);
                Assert.That(okResult.Content.IsSuccess, Is.True);
                Assert.That(okResult.Content.Data,Is.EqualTo(orderResponse));
            }

            /// <summary>
            /// PlaceOrder Function - InValid Request -Throws Exception
            /// </summary>
            [Test]
            public void PlaceOrder_InvalidRequest_ThrowsException()
            {
                var userId = Guid.NewGuid();
                var request = OrderTestUtil.CreateMockOrderRequestDto();

                _orderService
                    .Setup(x => x.PlaceOrderAsync(userId, request))
                    .ThrowsAsync(new Exception(ExceptionMessages.AtleastOneOrderItemRequired));

                SetUser(userId);

                Func<Task> Action = async () => await _orderController.PlaceOrderAsync(request);
                var exception = Assert.CatchAsync<Exception>(Action);

                Assert.That(exception.Message,Is.EqualTo(ExceptionMessages.AtleastOneOrderItemRequired));
            }

            /// <summary>
            /// GetOrderDetails Function - Valid Order Id - Returns Success
            /// </summary>
            [Test]
            public async Task GetOrderDetails_ValidOrderId_ReturnsSuccess()
            {
                var OrderId = Guid.NewGuid();
                var orderResponse = new OrderDetailsResponseDto();

                _orderService
                    .Setup(x => x.GetOrderDetailsAsync(OrderId))
                    .ReturnsAsync(orderResponse);

                var result =await _orderController.GetOrderDetailsAsync(OrderId);

                var okResult = result as OkNegotiatedContentResult<ApiResponseDto<OrderDetailsResponseDto>>;

                Assert.That(okResult, Is.Not.Null);
                Assert.That(okResult.Content.IsSuccess, Is.True);
                Assert.That(okResult.Content.Data,Is.EqualTo(orderResponse));
            }

            /// <summary>
            /// GetOrderDetails Function - InValid Order Id - Throws Exception
            /// </summary>
            [Test]
            public void GetOrderDetails_InvalidOrderId_ThrowsException()
            {
                var OrderId = Guid.NewGuid();

                _orderService
                    .Setup(x => x.GetOrderDetailsAsync(OrderId))
                    .ThrowsAsync(new Exception(ExceptionMessages.OrderNotFound));

                Func<Task> Action = async () => await _orderController.GetOrderDetailsAsync(OrderId);
                var exception = Assert.CatchAsync<Exception>(Action);

                Assert.That(exception.Message,Is.EqualTo(ExceptionMessages.OrderNotFound));
            }

            /// <summary>
            /// CancelOrder Function - Valid Order Id - Returns Success
            /// </summary>
            [Test]
            public async Task CancelOrder_ValidOrderId_ReturnsSuccess()
            {
                var orderId = Guid.NewGuid();
                var userId = Guid.NewGuid();

                SetUser(userId);

                _orderService
                    .Setup(x => x.CancelOrderAsync(orderId, userId))
                    .Returns(Task.CompletedTask);

                var result = await _orderController.CancelOrderASync(orderId);

                var okResult = result as OkNegotiatedContentResult<ApiResponseDto<OrderDetailsResponseDto>>;

                Assert.That(okResult, Is.Not.Null);
                Assert.That(okResult.Content.IsSuccess, Is.True);
                Assert.That(okResult.Content.Message,Is.EqualTo(SuccessMessages.OrderCancelledSuccessfully));
            }

            /// <summary>
            /// CancelOrder Function - InValid Order Id - Throws Exception
            /// </summary>
            [Test]
            public void CancelOrder_InvalidOrderId_ThrowsException()
            {
                var OrderId = Guid.NewGuid();
                var userId = Guid.NewGuid();

                SetUser(userId);

                _orderService
                    .Setup(x => x.CancelOrderAsync(OrderId, userId))
                    .ThrowsAsync(new Exception(ExceptionMessages.OrderNotFound));

                Func <Task> Action = async () => await _orderController.CancelOrderASync(OrderId);

                var exception = Assert.CatchAsync<Exception>(Action);

                Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.OrderNotFound));
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

                _orderController.ControllerContext = new HttpControllerContext();
                _orderController.User = new ClaimsPrincipal(identity);
            }
        }
    }
}
