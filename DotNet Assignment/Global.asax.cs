using DotNet_Assignment.App_Start;
using System.ComponentModel;
using System.Web.Http;
using Unity.AspNet.WebApi;

namespace DotNet_Assignment
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
           GlobalConfiguration.Configure(WebApiConfig.Register);

           UnityConfig.RegisterComponents();
        }
    }
}
