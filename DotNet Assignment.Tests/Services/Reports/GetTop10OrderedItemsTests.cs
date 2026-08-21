using DotNet_Assignment.Data;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Repository.Address;
using DotNet_Assignment.Repository.Orders;
using DotNet_Assignment.Repository.Reports;
using DotNet_Assignment.Repository.Restaurants;
using DotNet_Assignment.Repository.Users;
using DotNet_Assignment.Services.Orders;
using DotNet_Assignment.Services.Reports;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet_Assignment.Tests.Services.Reports
{
    [TestFixture]
    public class GetTop10OrderedItemsTests
    {
        private Mock<IReportRepository> _reportRepository;
        private Mock<IReportRenderer> _reportRenderer;
        private ReportService _reportService;

        [SetUp]
        public void Setup()
        {
            _reportRepository = new Mock<IReportRepository>();
            _reportRenderer = new Mock<IReportRenderer>();

            _reportService = new ReportService(
                _reportRepository.Object,
                _reportRenderer.Object);
        }

        /// <summary>
        /// GetTop10OrderedItemsAsync function - Valid Request - Returns Pdf
        /// </summary>
        [Test]
        public async Task GetTop10OrderedItemsAsync_ValidRequest_ReturnsPdf()
        {
            var ownerId = Guid.NewGuid();

            var category = "Veg";

            var data = new List<TopOrderedItemsResponseDto>
            {
                new TopOrderedItemsResponseDto
                {
                    MenuItemName = "Dish1",
                    TotalQuantity = 10
                },
                new TopOrderedItemsResponseDto
                {
                    MenuItemName = "Dish2",
                    TotalQuantity = 8
                }
            };

            var expectedPdf = new byte[] { 1, 2, 3 };

            _reportRepository
                .Setup(x => x.GetTop10OrderedItemsAsync(ownerId, category, It.IsAny<ExcludedItemsDto>()))
                .ReturnsAsync(data);

            _reportRenderer
                .Setup(x => x.RenderReport("~/Reports/Top10OrderedItems.trdp", data))
                .Returns(expectedPdf);

            var result = await _reportService.GetTop10OrderedItemsAsync(ownerId, category, It.IsAny<ExcludedItemsDto>());

            Assert.That(result, Is.EqualTo(expectedPdf));
        }

        /// <summary>
        /// GetTop10OrderedItemsAsync function - Valid Request - Calls Repository Once
        /// </summary>
        [Test]
        public async Task GetTop10OrderedItemsAsync_ValidRequest_CallsRepositoryOnce()
        {
            var ownerId = Guid.NewGuid();

            var category = "Veg";

            var data = new List<TopOrderedItemsResponseDto>();

            _reportRepository
                .Setup(x => x.GetTop10OrderedItemsAsync(ownerId, category, It.IsAny<ExcludedItemsDto>()))
                .ReturnsAsync(data);

            _reportRenderer
                .Setup(x => x.RenderReport("~/Reports/Top10OrderedItems.trdp", data))
                .Returns(new byte[] { 1 });

            await _reportService.GetTop10OrderedItemsAsync(ownerId, category, It.IsAny<ExcludedItemsDto>());

            _reportRepository.Verify(x => x.GetTop10OrderedItemsAsync(ownerId, category, It.IsAny<ExcludedItemsDto>()), Times.Once);
        }

        /// <summary>
        /// GetTop10OrderedItemsAsync function - Valid Request - Calls Renderer Once
        /// </summary>
        [Test]
        public async Task GetTop10OrderedItemsAsync_ValidRequest_CallsRendererOnce()
        {
            var ownerId = Guid.NewGuid();

            var category = "NonVeg";

            var data = new List<TopOrderedItemsResponseDto>
            {
                new TopOrderedItemsResponseDto
                {
                    MenuItemName = "Dish1",
                    TotalQuantity = 20
                }
            };

            _reportRepository
                .Setup(x => x.GetTop10OrderedItemsAsync(ownerId, category, It.IsAny<ExcludedItemsDto>()))
                .ReturnsAsync(data);

            _reportRenderer
                .Setup(x => x.RenderReport("~/Reports/Top10OrderedItems.trdp", data))
                .Returns(new byte[] { 1 });

            await _reportService.GetTop10OrderedItemsAsync(ownerId, category, It.IsAny<ExcludedItemsDto>());

            _reportRenderer.Verify(x => x.RenderReport("~/Reports/Top10OrderedItems.trdp", data), Times.Once);
        }
    }
}
