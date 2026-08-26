using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using DotNet_Assignment.Handlers;
using Newtonsoft.Json;

namespace DotNet_Assignment
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Services.Replace(
                typeof(IExceptionHandler), new GlobalExceptionHandler());

            config.Filters.Add(new ModelStateHandler());

            config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        }
    }
}
