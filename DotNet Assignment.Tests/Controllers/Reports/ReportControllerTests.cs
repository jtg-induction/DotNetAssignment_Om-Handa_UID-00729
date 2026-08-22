using DotNet_Assignment.Controllers;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Services.Orders;
using DotNet_Assignment.Services.Reports;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace DotNet_Assignment.Tests.Controllers.Reports
{
    [TestFixture]
    public class ReportControllerTests
    {
        private Mock<IReportService> _reportService;
        private ReportController _reportController;

        [SetUp]
        public void Setup()
        {
            _reportService = new Mock<IReportService>();

            _reportController = new ReportController(_reportService.Object);
        }

        /// <summary>
        /// GetTop10OrderedItemsAsync function - Valid Request - Calls Service Once
        /// </summary>
        [Test]
        public async Task GetTop10OrderedItemsAsync_ValidRequest_ReturnsPdf()
        {
            var excludedItems = new ExcludedItemsDto { ExcludeItems = new List<string> { "Dish1" } };
            var pdf = new byte[] { 1, 2, 3 };

            var userId = Guid.NewGuid();
            SetUser(userId);

            _reportService
                .Setup(x => x.GetTop10OrderedItemsAsync(userId, "Veg", excludedItems))
                .ReturnsAsync(pdf);

            var result = await _reportController.GetTop10OrderedItemsAsync(excludedItems, "Veg");

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Content, Is.TypeOf<ByteArrayContent>());
        }

        /// <summary>
        /// GetTop10OrderedItemsAsync function - Valid Request - Calls Service Once
        /// </summary>
        [Test]
        public async Task GetTop10OrderedItemsAsync_ValidRequest_CallsServiceOnce()
        {
            var excludedItems = new ExcludedItemsDto { ExcludeItems = new List<string>() };
            var pdf = new byte[] { 1, 2, 3 };

            var userId = Guid.NewGuid();
            SetUser(userId);

            _reportService
                .Setup(x => x.GetTop10OrderedItemsAsync(userId, "NonVeg", excludedItems))
                .ReturnsAsync(pdf);

            await _reportController.GetTop10OrderedItemsAsync(excludedItems, "NonVeg");

            _reportService.Verify(x => x.GetTop10OrderedItemsAsync(userId, "NonVeg", excludedItems), Times.Once);
        }

        /// <summary>
        /// GetTop10OrderedItemsAsync function - Valid Request - Returns Correct Headers
        /// </summary>
        [Test]
        public async Task GetTop10OrderedItemsAsync_ValidRequest_ReturnsCorrectHeaders()
        {
            var excludedItems = new ExcludedItemsDto { ExcludeItems = new List<string>() };
            var pdf = new byte[] { 1, 2, 3 };

            var userId = Guid.NewGuid();
            SetUser(userId);

            _reportService
                .Setup(x => x.GetTop10OrderedItemsAsync(userId, null, excludedItems))
                .ReturnsAsync(pdf);

            var result = await _reportController.GetTop10OrderedItemsAsync(excludedItems);

            Assert.That(result.Content.Headers.ContentType.MediaType, Is.EqualTo("application/pdf"));
            Assert.That(result.Content.Headers.ContentDisposition.FileName, Is.EqualTo("Top10OrderedItems.pdf"));
        }

        /// <summary>
        /// GetTop10OrderedItemsAsync function - Service Throws - Throws Exception
        /// </summary>
        [Test]
        public void GetTop10OrderedItemsAsync_ServiceThrows_ThrowsException()
        {
            var excludedItems = new ExcludedItemsDto { ExcludeItems = new List<string>() };

            var userId = Guid.NewGuid();
            SetUser(userId);

            _reportService
                .Setup(x => x.GetTop10OrderedItemsAsync(userId, "Veg", excludedItems))
                .ThrowsAsync(new Exception());

            Func<Task> Action = async () => await _reportController.GetTop10OrderedItemsAsync(excludedItems, "Veg");

            Assert.ThrowsAsync<Exception>(Action);
        }

        /// <summary>
        /// FrequentlyBoughtTogether function - Valid Request - Returns Pdf
        /// </summary>
        [Test]
        public async Task FrequentlyBoughtTogether_ValidRequest_ReturnsPdf()
        {
            var pdf = new byte[] { 4, 5, 6 };

            var userId = Guid.NewGuid();
            SetUser(userId);

            _reportService
                .Setup(x => x.FrequentlyBoughtTogetherAsync(userId, 5, It.IsAny<IncludedRestaurantsDto>()))
                .ReturnsAsync(pdf);

            var result = await _reportController.FrequentlyBoughtTogetherAsync(new IncludedRestaurantsDto { }, 5);

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(await result.Content.ReadAsByteArrayAsync(), Is.EqualTo(pdf));
        }

        /// <summary>
        /// FrequentlyBoughtTogether function - Valid Request - Calls Service Once
        /// </summary>
        [Test]
        public async Task FrequentlyBoughtTogether_ValidRequest_CallsServiceOnce()
        {
            var pdf = new byte[] { 4, 5, 6 };

            var userId = Guid.NewGuid();
            SetUser(userId);

            _reportService
                .Setup(x => x.FrequentlyBoughtTogetherAsync(userId, 10, It.IsAny<IncludedRestaurantsDto>()))
                .ReturnsAsync(pdf);

            await _reportController.FrequentlyBoughtTogetherAsync(new IncludedRestaurantsDto { }, 10);

            _reportService.Verify(x => x.FrequentlyBoughtTogetherAsync(userId, 10, It.IsAny<IncludedRestaurantsDto>()), Times.Once);
        }

        /// <summary>
        /// FrequentlyBoughtTogether function - Valid Request - Returns Correct Headers
        /// </summary>
        [Test]
        public async Task FrequentlyBoughtTogether_ValidRequest_ReturnsCorrectHeaders()
        {
            var pdf = new byte[] { 4, 5, 6 }; 
            
            var userId = Guid.NewGuid();
            SetUser(userId);

            _reportService
                .Setup(x => x.FrequentlyBoughtTogetherAsync(userId, 5, It.IsAny<IncludedRestaurantsDto>()))
                .ReturnsAsync(pdf);

            var result = await _reportController.FrequentlyBoughtTogetherAsync(new IncludedRestaurantsDto { }, 5);

            Assert.That(result.Content.Headers.ContentType.MediaType, Is.EqualTo("application/pdf"));
            Assert.That(result.Content.Headers.ContentDisposition.FileName, Is.EqualTo("FrequentlyBoughtTogether.pdf"));
        }

        /// <summary>
        /// FrequentlyBoughtTogether function - Service Throws - Throws Exception
        /// </summary>
        [Test]
        public void FrequentlyBoughtTogether_ServiceThrows_ThrowsException()
        {
            var userId = Guid.NewGuid();
            SetUser(userId);

            _reportService
                .Setup(x => x.FrequentlyBoughtTogetherAsync(userId, 5, It.IsAny<IncludedRestaurantsDto>()))
                .ThrowsAsync(new Exception());

            Func<Task> Action = async () => await _reportController.FrequentlyBoughtTogetherAsync(new IncludedRestaurantsDto { }, 5);
            Assert.ThrowsAsync<Exception>(Action);
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

            _reportController.ControllerContext = new HttpControllerContext();
            _reportController.User = new ClaimsPrincipal(identity);
        }
    }
}
