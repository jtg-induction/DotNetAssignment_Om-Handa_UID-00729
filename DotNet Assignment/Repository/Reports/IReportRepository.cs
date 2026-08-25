using DotNet_Assignment.Models.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNet_Assignment.Repository.Reports
{
    public interface IReportRepository
    {
        Task<List<TopOrderedItemsResponseDto>> GetTop10OrderedItemsAsync(Guid ownerId, string category, TopOrderedItemsRequestDto excludedItemsDto);

        Task<List<FrequentlyBoughtItemsDto>> GetFrequentlyBoughtTogetherAsync(Guid ownerId,IncludedRestaurantsDto includedRestaurants, int? size );

    }
}
