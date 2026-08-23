using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Repository.Reports;
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
    public class FrequentlyBoughtTogetherTests
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
        /// FrequentlyBoughtTogetherAsync function - Valid Request - Returns Pdf
        /// </summary>
        [Test]
        public async Task FrequentlyBoughtTogetherAsync_ValidRequest_ReturnsPdf()
        {
            var ownerId = Guid.NewGuid();

            var data = new List<FrequentlyBoughtItemsDto>
            {
                new FrequentlyBoughtItemsDto
                {
                    Item1 = "Dish1",
                    Item2 = "Dish2",
                    TotalTimesBought = 10
                }
            };

            var expectedPdf = new byte[] { 1, 2, 3 };

            _reportRepository
                .Setup(x => x.GetFrequentlyBoughtTogetherAsync(ownerId, It.IsAny<IncludedRestaurantsDto>(), 5))
                .ReturnsAsync(data);

            _reportRenderer
                .Setup(x => x.RenderReport("~/Reports/FrequentlyBoughtTogether.trdp", data))
                .Returns(expectedPdf);

            var result = await _reportService.FrequentlyBoughtTogetherAsync(ownerId, It.IsAny<IncludedRestaurantsDto>(), 5);

            Assert.That(result, Is.EqualTo(expectedPdf));
        }

        /// <summary>
        /// FrequentlyBoughtTogetherAsync function - Valid Request - Calls Repository Once
        /// </summary>
        [Test]
        public async Task FrequentlyBoughtTogetherAsync_ValidRequest_CallsRepositoryOnce()
        {
            var ownerId = Guid.NewGuid();

            var data = new List<FrequentlyBoughtItemsDto>();

            _reportRepository
                .Setup(x => x.GetFrequentlyBoughtTogetherAsync(ownerId, It.IsAny<IncludedRestaurantsDto>(), 5))
                .ReturnsAsync(data);

            _reportRenderer
                .Setup(x => x.RenderReport("~/Reports/FrequentlyBoughtTogether.trdp", data))
                .Returns(new byte[] { 1 });

            await _reportService.FrequentlyBoughtTogetherAsync(ownerId, It.IsAny<IncludedRestaurantsDto>(), 5);

            _reportRepository.Verify(x => x.GetFrequentlyBoughtTogetherAsync(ownerId, It.IsAny<IncludedRestaurantsDto>(), 5), Times.Once);
        }

        /// <summary>
        /// FrequentlyBoughtTogetherAsync function - Valid Request - Calls Renderer Once
        /// </summary>
        [Test]
        public async Task FrequentlyBoughtTogetherAsync_ValidRequest_CallsRendererOnce()
        {
            var ownerId = Guid.NewGuid();

            var data = new List<FrequentlyBoughtItemsDto>
            {
                new FrequentlyBoughtItemsDto
                {
                    Item1 = "Dish1",
                    Item2 = "Dish2",
                    TotalTimesBought = 10
                }
            };

            _reportRepository
                .Setup(x => x.GetFrequentlyBoughtTogetherAsync(ownerId, It.IsAny<IncludedRestaurantsDto>(), 5))
                .ReturnsAsync(data);

            _reportRenderer
                .Setup(x => x.RenderReport("~/Reports/FrequentlyBoughtTogether.trdp", data))
                .Returns(new byte[] { 1 });

            await _reportService.FrequentlyBoughtTogetherAsync(ownerId, It.IsAny<IncludedRestaurantsDto>(), 5);

            _reportRenderer.Verify(x => x.RenderReport("~/Reports/FrequentlyBoughtTogether.trdp", data), Times.Once);
        }
    }
}
