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
            var container = new UnityContainer();

            container.RegisterType<AppDbContext>(new HierarchicalLifetimeManager());

<<<<<<< HEAD
            Container.RegisterType<IAuthService, AuthService>();
            Container.RegisterType<IJWTService, JWTService>();
            Container.RegisterType<IUserService, UserService>();
=======
            container.RegisterType<IAuthService, AuthService>(new HierarchicalLifetimeManager());
            container.RegisterType<IJWTService, JWTService>(new ContainerControlledLifetimeManager());
>>>>>>> b6f1693048ee4dbc96dc16cb9cea427cd08c5b37

            container.RegisterType<IUserRepository, UserRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IRefreshTokenRepository, RefreshTokenRepository>(new HierarchicalLifetimeManager());

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);

            return container;
        }
    }
}
