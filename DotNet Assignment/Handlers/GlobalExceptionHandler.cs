using DotNet_Assignment.Constants;
using DotNet_Assignment.Models.DTO;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
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

            var exception = context.Exception;
            var statusCode = HttpStatusCode.InternalServerError;

            switch (exception)
            {
                case KeyNotFoundException _:
                    statusCode = HttpStatusCode.NotFound;
                    break;

                case SecurityTokenExpiredException _:
                case SecurityTokenException _:
                case AuthenticationException _:
                    statusCode = HttpStatusCode.Unauthorized;
                    break;

                case InvalidOperationException _ when exception.Message == ExceptionMessages.UserDeactivated:
                case UnauthorizedAccessException _:
                    statusCode = HttpStatusCode.Forbidden;
                    break;

                case InvalidOperationException _ when exception.Message == ExceptionMessages.EmailAlreadyExists:
                    statusCode = HttpStatusCode.Conflict;
                    break;

                case InvalidOperationException _:
                case ArgumentException _:
                    statusCode = HttpStatusCode.BadRequest;
                    break;

            }

            var response = new ApiResponseDto<object>
            {
                IsSuccess = false,
                Message = exception.Message,
            };

            context.Result = new ResponseMessageResult(context.Request.CreateResponse(statusCode, response));

            return Task.CompletedTask;
        }
    }
}
