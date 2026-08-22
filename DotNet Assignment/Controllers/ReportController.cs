using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Enums;
using DotNet_Assignment.Services.Auth;
using DotNet_Assignment.Services.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;

namespace DotNet_Assignment.Controllers
{
    [RoutePrefix("api/reports")]
    public class ReportController : ApiController
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        /// <summary>
        /// Gets top 10 ordered items list
        /// </summary>
        /// <returns>Downloadable pdf bytes array</returns>
        [Authorize(Roles = nameof(UserRoles.Owner))]
        [HttpGet]
        [Route("top-items")]
        public async Task<HttpResponseMessage> GetTop10OrderedItemsAsync(ExcludedItemsDto excludedItemsDto, [FromUri] string category = null)
        {
            var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);

            var pdf = await _reportService.GetTop10OrderedItemsAsync(userId, category, excludedItemsDto);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(pdf)
            };

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "Top10OrderedItems.pdf"
            };

            return response;
        }

        /// <summary>
        /// Gets two frequently bought items list
        /// </summary>
        /// <returns>Downloadable pdf bytes array</returns>
        [Authorize(Roles = nameof(UserRoles.Owner))]
        [HttpGet]
        [Route("bought-together")]
        public async Task<HttpResponseMessage> FrequentlyBoughtTogetherAsync(IncludedRestaurantsDto includedRestaurants, [FromUri]int size = 5)
        {
            var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);

            var pdf = await _reportService.FrequentlyBoughtTogetherAsync(userId, size, includedRestaurants);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(pdf)
            };

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "FrequentlyBoughtTogether.pdf"
            };

            return response;
        }
    }
}
