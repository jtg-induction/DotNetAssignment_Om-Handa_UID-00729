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
using DotNet_Assignment.Services.Address;
using DotNet_Assignment.Repository.Address;
using DotNet_Assignment.Services.Restaurants;
using DotNet_Assignment.Repository.Restaurants;
using DotNet_Assignment.Services.Orders;
using DotNet_Assignment.Repository.Orders;

namespace DotNet_Assignment.App_Start
{
    public class UnityConfig
    {
        /// <summary>
        /// Registers Dependencies and configures them globally
        /// </summary>
        public static IUnityContainer RegisterComponents()
        {
            var container = new UnityContainer();

            container.RegisterType<AppDbContext>(new HierarchicalLifetimeManager());

            container.RegisterType<IAuthService, AuthService>(new HierarchicalLifetimeManager());
            container.RegisterType<IJWTService, JWTService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IUserService, UserService>(new HierarchicalLifetimeManager());
            container.RegisterType<IAddressService, AddressService>(new HierarchicalLifetimeManager());
            container.RegisterType<IRestaurantService, RestaurantService>(new HierarchicalLifetimeManager());
            container.RegisterType<IOrderService, OrderService>(new HierarchicalLifetimeManager());

            container.RegisterType<IUserRepository, UserRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IRefreshTokenRepository, RefreshTokenRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IAddressRepository, AddressRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IRestaurantRepository, RestaurantRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IOrderRepository, OrderRepository>(new HierarchicalLifetimeManager());

            GlobalConfiguration.Configuration.DependencyResolver = new UnityHierarchicalDependencyResolver(container);

            return container;
        }
    }
}
