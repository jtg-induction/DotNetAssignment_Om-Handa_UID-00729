using DotNet_Assignment.Models.DTO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace DotNet_Assignment.Handlers
{
    public class ModelStateHandler :ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid) {
                var errors = actionContext.ModelState.Values
                    .SelectMany(x=>x.Errors)
                    .Select(x=>x.ErrorMessage)
                    .ToList();

                var response = new ApiResponseDto<object> { IsSuccess = false, Message = "Invalid Credentials", Data = errors };

                actionContext.Response= actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, response);
                return;
            }
            base.OnActionExecuting(actionContext);
        }
    }
}
