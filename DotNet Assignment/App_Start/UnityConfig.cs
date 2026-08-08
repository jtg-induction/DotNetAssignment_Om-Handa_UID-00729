using DotNet_Assignment.Data;
using DotNet_Assignment.Repository.Users;
using DotNet_Assignment.Services.Auth;
using System.Web.Http;
using Unity;
using Unity.Lifetime;
using Unity.AspNet.WebApi;
using DotNet_Assignment.Services.JWT;
using DotNet_Assignment.Repository.RefreshTokens;

namespace DotNet_Assignment.App_Start
{
    public class UnityConfig
    {
        public static IUnityContainer RegisterComponents()
        {
            var container = new UnityContainer();

            container.RegisterType<AppDbContext>(new HierarchicalLifetimeManager());

            container.RegisterType<IAuthService, AuthService>(new HierarchicalLifetimeManager());
            container.RegisterType<IJWTService, JWTService>(new ContainerControlledLifetimeManager());

            container.RegisterType<IUserRepository, UserRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IRefreshTokenRepository, RefreshTokenRepository>(new HierarchicalLifetimeManager());

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);

            return container;
        }
    }
}
