using DotNet_Assignment.Constants;
using DotNet_Assignment.Data;
using DotNet_Assignment.Exceptions;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Models.Enums;
using DotNet_Assignment.Repository.Address;
using DotNet_Assignment.Repository.Orders;
using DotNet_Assignment.Repository.Restaurants;
using DotNet_Assignment.Repository.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet_Assignment.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly AppDbContext _appDbContext;

        public OrderService(
            IOrderRepository orderRepository,
            IRestaurantRepository restaurantRepository,
            IAddressRepository addressRepository,
            IUserRepository userRepository,
            AppDbContext appDbContext
            )
        {
            _orderRepository = orderRepository;
            _restaurantRepository = restaurantRepository;
            _addressRepository = addressRepository;
            _userRepository = userRepository;
            _appDbContext = appDbContext;
        }

        /// <summary>
        /// Places an order
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orderRequestDto"></param>
        /// <returns>Order response including total price, order id</returns>
        /// <exception cref="Exception">if user, restaurant, address, menuitem not found, user deactivated or no menuitem in requestDto</exception>
        public async Task<OrderResponseDto> PlaceOrderAsync(Guid userId, OrderRequestDto orderRequestDto)
        {
            using (var transaction = _appDbContext.Database.BeginTransaction())
            {
                var response = await ExecutePlaceOrderAsync(userId, orderRequestDto);

                transaction.Commit();

                return response;
            }
        }

        public async Task<OrderResponseDto> ExecutePlaceOrderAsync(Guid userId, OrderRequestDto orderRequestDto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new KeyNotFoundException(ExceptionMessages.UserNotFound);
            }

            if (user.IsDeleted)
            {
                throw new UserDeactivatedException(ExceptionMessages.UserDeactivated);
            }

            var restaurant = await _restaurantRepository.GetRestaurantByIdAsync(orderRequestDto.RestaurantId);

            if (restaurant == null)
            {
                throw new KeyNotFoundException(ExceptionMessages.RestaurantNotFound);
            }

            if (!restaurant.IsOpen)
            {
                throw new WrongOperationException(ExceptionMessages.RestaurantIsClosed);
            }

            if (orderRequestDto.OrderedItems == null)
            {
                throw new ArgumentException(ExceptionMessages.AtleastOneOrderItemRequired);
            }

            var address = await _addressRepository.GetAddressByIdAsync(orderRequestDto.AddressId, userId);

            if (address == null)
            {
                throw new KeyNotFoundException(ExceptionMessages.AddressNotFound);
            }

            string deliveryAddress = address.HouseNumber + ", " + address.Street + ", " + address.City + ", " + address.State + ", " + address.Pincode + ", " + address.Landmark;

            var order = new Order
            {
                DeliveryAddress = deliveryAddress,
                UserId = userId,
                RestaurantId = orderRequestDto.RestaurantId
            };

            var orderedItemIds = order.OrderedItems.Select(oi => oi.MenuItemId).ToList();

            var menuItems = await _restaurantRepository.GetMenuItemsForUpdateAsync(orderedItemIds);

            decimal totalPrice = 0m;

            foreach (OrderItemDto item in orderRequestDto.OrderedItems)
            {

                var menuItem = menuItems.FirstOrDefault(mi => mi.MenuItemId == item.MenuItemId);

                if (menuItem == null)
                {
                    throw new WrongOperationException(ExceptionMessages.MenuItemNotFound);
                }

                if (menuItem.IsDeleted || menuItem.RestaurantId != orderRequestDto.RestaurantId)
                {
                    throw new WrongOperationException(ExceptionMessages.MenuItemNotFound);
                }

                if (menuItem.QuantityAvailable < 0 || (menuItem.QuantityAvailable - item.Quantity) < 0)
                {
                    throw new WrongOperationException(ExceptionMessages.InsufficientStock);
                }

                if ((user.Balance - (menuItem.Price * item.Quantity)) < 0)
                {
                    throw new WrongOperationException(ExceptionMessages.InsufficientBalance);
                }

                menuItem.QuantityAvailable -= item.Quantity;
                user.Balance = user.Balance - (menuItem.Price * item.Quantity);

                var orderItem = new OrderedItem
                {
                    Quantity = item.Quantity,
                    ItemPrice = menuItem.Price,
                    OrderId = order.OrderId,
                    MenuItemId = menuItem.MenuItemId
                };

                totalPrice += menuItem.Price * item.Quantity;

                _orderRepository.AddOrderItem(orderItem);
            }

            order.TotalPrice = totalPrice;

            _orderRepository.AddOrder(order);

            await _appDbContext.SaveChangesAsync();

            return new OrderResponseDto
            {
                OrderId = order.OrderId
            };
        }

        /// <summary>
        /// Get order details of a specific order
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>Order Details</returns>
        /// <exception cref="Exception">if no order found</exception>
        public async Task<OrderDetailsResponseDto> GetOrderDetailsAsync(Guid orderId, Guid userId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);

            if (order == null)
            {
                throw new KeyNotFoundException(ExceptionMessages.OrderNotFound);
            }

            var orderUserId = order.UserId;

            if (userId != orderUserId)
            {
                throw new UnauthorizedAccessException(ExceptionMessages.Unauthorized);
            }

            var orderedItems = await _orderRepository.GetOrderItemsAsync(orderId);

            var restaurant = await _restaurantRepository.GetRestaurantByIdAsync(order.RestaurantId);

            return new OrderDetailsResponseDto
            {
                OrderId = orderId,
                TotalPrice = order.TotalPrice,
                Status = order.Status.ToString(),
                DeliveryAddress = order.DeliveryAddress,
                RestaurantName = restaurant.Name,

                OrderedItems = orderedItems.Select(item => new MenuDetailsResponseDto
                {
                    Name = item.MenuItem.Name,
                    Description = item.MenuItem.Description,
                    Price = item.ItemPrice,
                    Quantity = item.Quantity
                }).ToList()
            };
        }

        /// <summary>
        /// Cancels a order by its order ID
        /// </summary>
        /// <param name="orderId"></param>
        /// <exception cref="Exception">If order not found or already cancelled/exception>
        public async Task CancelOrderAsync(Guid orderId, Guid userId)
        {
            using (var transaction = _appDbContext.Database.BeginTransaction())
            {

                await ExecuteCancelOrderAsync(orderId, userId);
                transaction.Commit();

            }
        }

        public async Task ExecuteCancelOrderAsync(Guid orderId, Guid userId)
        {
            var order = await _orderRepository.GetOrderForUpdateAsync(orderId);

            if (order == null)
            {
                throw new KeyNotFoundException(ExceptionMessages.OrderNotFound);
            }

            var orderUserId = order.UserId;

            if (userId != orderUserId)
            {
                throw new UnauthorizedAccessException(ExceptionMessages.Unauthorized);
            }

            var user = await _userRepository.GetUserByIdAsync(orderUserId);

            if (user == null)
            {
                throw new KeyNotFoundException(ExceptionMessages.UserNotFound);
            }

            if (user.IsDeleted)
            {
                throw new UserDeactivatedException(ExceptionMessages.UserDeactivated);
            }

            if (order.Status == OrderStatus.Cancelled)
            {
                throw new WrongOperationException(ExceptionMessages.OrderAlreadyCancelled);
            }

            if (order.Status == OrderStatus.Rejected)
            {
                throw new WrongOperationException(ExceptionMessages.OrderRejectedByRestaurant);
            }

            if (order.Status == OrderStatus.Dispatched || order.Status == OrderStatus.Delivered)
            {
                throw new WrongOperationException(ExceptionMessages.OrderCantBeCancelled);
            }

            var orderedItemIds = order.OrderedItems.Select(oi => oi.MenuItemId).ToList();

            var menuItems = await _restaurantRepository.GetMenuItemsForUpdateAsync(orderedItemIds);

            foreach (OrderedItem orderedItem in order.OrderedItems)
            {
                var menuItem = menuItems.FirstOrDefault(mi => mi.MenuItemId == orderedItem.MenuItemId);

                menuItem.QuantityAvailable += orderedItem.Quantity;
            }

            user.Balance += order.TotalPrice;
            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;

            await _appDbContext.SaveChangesAsync();

        }

        /// <summary>
        /// Changes order status of an order based on order Id
        /// </summary>
        /// <param name="orderStatusDto"></param>
        /// <param name="orderId"></param>
        /// <exception cref="Exception">If order not found or new order status value is wrong</exception>
        public async Task ChangeOrderStatusAsync(ChangeOrderStatusDto orderStatusDto, Guid orderId, Guid ownerId)
        {
            if (!Enum.IsDefined(typeof(OrderStatus), orderStatusDto.Status.Value))
            {
                throw new ArgumentException(ExceptionMessages.InvalidOrderStatus);
            }

            var order = await _orderRepository.GetOrderForUpdateAsync(orderId);

            if (order == null)
            {
                throw new KeyNotFoundException(ExceptionMessages.OrderNotFound);
            }

            if (!order.Restaurant.RestaurantOwners.Any(x => x.UserId == ownerId))
            {
                throw new UnauthorizedAccessException(ExceptionMessages.Unauthorized);
            }

            if (order.Status == orderStatusDto.Status)
            {
                throw new InvalidOperationException(ExceptionMessages.OrderStatusAlreadyChanged);
            }

            if ((int)order.Status != (int)orderStatusDto.Status + 1)
            {
                throw new InvalidOperationException(ExceptionMessages.StatusCantBeChanged);
            }

            if ((int)orderStatusDto?.Status < (int)order.Status)
            {
                throw new InvalidOperationException(ExceptionMessages.StatusCantBeChanged);
            }

            if (orderStatusDto?.Status == OrderStatus.Cancelled)
            {
                throw new InvalidOperationException(ExceptionMessages.Unauthorized);
            }

            if (orderStatusDto?.Status == OrderStatus.Rejected)
            {
                await CancelOrderAsync(order.OrderId, order.UserId);
            }

            order.Status = orderStatusDto.Status.Value;

            await _appDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gets filtered orders based on options (query params) 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="filterOptions">Query params given by controller</param>
        /// <returns>List of orders</returns>
        /// <exception cref="Exception">if orders not found</exception>
        public async Task<List<OrderDetailsResponseDto>> GetFilteredOrders(Guid userId, FilterOptionsDto filterOptions)
        {
            var orders = await _orderRepository.FilterOrderAsync(userId, filterOptions);

            return orders.Select(o => new OrderDetailsResponseDto
            {
                OrderId = o.OrderId,
                TotalPrice = o.TotalPrice,
                Status = o.Status.ToString(),
                DeliveryAddress = o.DeliveryAddress,
                RestaurantName = o.Restaurant.Name,

                OrderedItems = o.OrderedItems.Select(oi => new MenuDetailsResponseDto
                {
                    Name = oi.MenuItem.Name,
                    Description = oi.MenuItem.Description,
                    Price = oi.ItemPrice,
                    Quantity = oi.Quantity
                }).ToList()
            }).ToList();
        }
    }
}
