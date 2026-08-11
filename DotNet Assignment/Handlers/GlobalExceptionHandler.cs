using DotNet_Assignment.Models.DTO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace DotNet_Assignment.Handlers
{
    public class GlobalExceptionHandler : ExceptionHandler
    {
        /// <summary>
        /// Handles all Exceptions and return them as ApiResponseDto
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>HTTP request with exception message</returns>
        public override Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            var response = new ApiResponseDto<object>
            {
                IsSuccess = false,
                Message = context.Exception.Message,
            };

            context.Result = new ResponseMessageResult(context.Request.CreateResponse(HttpStatusCode.BadRequest, response));

            return Task.CompletedTask;
        }
    }
}
