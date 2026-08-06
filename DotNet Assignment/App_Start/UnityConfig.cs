using DotNet_Assignment.Data;
using DotNet_Assignment.Repository.Users;
using DotNet_Assignment.Services.PasswordService;
using DotNet_Assignment.Services.Auth;
using System.Web.Http;
using Unity;
using Unity.AspNet.WebApi;
using DotNet_Assignment.Services.JWT;
using DotNet_Assignment.Repository.RefreshTokens;
using DotNet_Assignment.Services.Users;

namespace DotNet_Assignment.App_Start
{
    public class UnityConfig
    {
        public static IUnityContainer RegisterComponents()
        {
            var Container = new UnityContainer();

            Container.RegisterType<AppDbContext>();

            Container.RegisterType<IPasswordService, PasswordService>();
            Container.RegisterType<IAuthService, AuthService>();
            Container.RegisterType<IJWTService, JWTService>();
            Container.RegisterType<IUserService, UserService>();

            Container.RegisterType<IUserRepository, UserRepository>();
            Container.RegisterType<IRefreshTokenRepository, RefreshTokenRepository>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(Container);

            return Container;
        }
    }
}
