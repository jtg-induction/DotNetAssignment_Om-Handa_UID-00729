using DotNet_Assignment.Data;
using DotNet_Assignment.Repository.Users;
using DotNet_Assignment.Services.Auth;
using System.Web.Http;
using Unity;
using Unity.Lifetime;
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

            Container.RegisterType<AppDbContext>(new HierarchicalLifetimeManager());

            Container.RegisterType<IAuthService, AuthService>();
            Container.RegisterType<IJWTService, JWTService>();
            Container.RegisterType<IUserService, UserService>();

            Container.RegisterType<IUserRepository, UserRepository>(new HierarchicalLifetimeManager());
            Container.RegisterType<IRefreshTokenRepository, RefreshTokenRepository>(new HierarchicalLifetimeManager());

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(Container);

            return Container;
        }
    }
}
