using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Repository.Reports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using Telerik.Reporting;
using Telerik.Reporting.Processing;

namespace DotNet_Assignment.Services.Reports
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IReportRenderer _reportRenderer;

        public ReportService(IReportRepository reportRepository, IReportRenderer reportRenderer)
        {
            _reportRepository = reportRepository;
            _reportRenderer = reportRenderer;
        }

        /// <summary>
        /// Gets Top 10 orderd items from Repository and Added DataSource to report
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns>Pdf bytes </returns>
        public async Task<byte[]> GetTop10OrderedItemsAsync(Guid ownerId, string category, ExcludedItemsDto excludedItemsDto)
        {
            var data = await _reportRepository.GetTop10OrderedItemsAsync(ownerId, category, excludedItemsDto);

            return _reportRenderer.RenderReport("~/Reports/Top10OrderedItems.trdp", data);
        }

        /// <summary>
        /// Gets frequently bought items from Repository and Added DataSource to report
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns>Pdf bytes </returns>
        public async Task<byte[]> FrequentlyBoughtTogetherAsync(Guid ownerId, int size, IncludedRestaurantsDto includedRestaurants)
        {
            var data = await _reportRepository.GetFrequentlyBoughtTogetherAsync(ownerId, size, includedRestaurants);

            return _reportRenderer.RenderReport("~/Reports/FrequentlyBoughtTogether.trdp", data);
        }
    }
}
