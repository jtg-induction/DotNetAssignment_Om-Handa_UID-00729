using DotNet_Assignment.Constants;
using DotNet_Assignment.Exceptions;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Repository.Reports;
using DotNet_Assignment.Repository.Restaurants;
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
        private readonly IRestaurantRepository _restaurantRepository;

        public ReportService(
            IReportRepository reportRepository, 
            IReportRenderer reportRenderer,
            IRestaurantRepository restaurantRepository
            )
        {
            _reportRepository = reportRepository;
            _reportRenderer = reportRenderer;
            _restaurantRepository = restaurantRepository;
        }

        /// <summary>
        /// Gets Top 10 orderd items from Repository and Added DataSource to report
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns>Pdf bytes </returns>
        public async Task<byte[]> GetTop10OrderedItemsAsync(Guid ownerId, string category, TopOrderedItemsRequestDto excludedItemsDto)
        {
            var restaurants = await _restaurantRepository.GetRestaurantsWithOwnersByIdsAsync(excludedItemsDto.IncludeRestaurants);

            foreach (var restaurant in restaurants)
            {
                if (!restaurant.RestaurantOwners.Any(x => x.UserId == ownerId))
                {
                    throw new KeyNotFoundException(ExceptionMessages.OwnerNotAssignedToRestaurant);
                }
            }

            var data = await _reportRepository.GetTop10OrderedItemsAsync(ownerId, category, excludedItemsDto);

            return _reportRenderer.RenderReport("~/Reports/Top10OrderedItems.trdp", data);
        }

        /// <summary>
        /// Gets frequently bought items from Repository and Added DataSource to report
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns>Pdf bytes </returns>
        public async Task<byte[]> FrequentlyBoughtTogetherAsync(Guid ownerId, IncludedRestaurantsDto includedRestaurants,int? size)
        {
            var restaurants = await _restaurantRepository.GetRestaurantsWithOwnersByIdsAsync(includedRestaurants.IncludeRestaurants);

            foreach(var restaurant in restaurants)
            {
                if (!restaurant.RestaurantOwners.Any(x => x.UserId == ownerId))
                {
                    throw new KeyNotFoundException(ExceptionMessages.OwnerNotAssignedToRestaurant);
                }
            }

            var data = await _reportRepository.GetFrequentlyBoughtTogetherAsync(ownerId, includedRestaurants, size);

            return _reportRenderer.RenderReport("~/Reports/FrequentlyBoughtTogether.trdp", data);
        }
    }
}
