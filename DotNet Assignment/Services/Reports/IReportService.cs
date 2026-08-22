using DotNet_Assignment.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet_Assignment.Services.Reports
{
    public interface IReportService
    {
        Task<byte[]> GetTop10OrderedItemsAsync(Guid ownerId, string category, ExcludedItemsDto excludedItemsDto);

        Task<byte[]> FrequentlyBoughtTogetherAsync(Guid ownerId, int size, IncludedRestaurantsDto includedRestaurants);
    }
}
