using DotNet_Assignment.Models.DTO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using DotNet_Assignment.Constants;

namespace DotNet_Assignment.Handlers
{
    public class ModelStateHandler :ActionFilterAttribute
    {
        /// <summary>
        /// Validates Model State on start of action's  and returns as ApiResponse 
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid) {
                var errors = actionContext.ModelState
                            .Where(x => x.Value.Errors.Count > 0)
                            .ToDictionary(
                                er => er.Key.Replace("requestDto.", ""),
                                er => er.Value.Errors.Select(e => e.ErrorMessage).ToList()
                            );

                var response = new ApiResponseDto<object> { IsSuccess = false, Message = ExceptionMessages.InvalidCredentials, Errors = errors };

                actionContext.Response= actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, response);
                return;
            }
            base.OnActionExecuting(actionContext);
        }
    }
}
