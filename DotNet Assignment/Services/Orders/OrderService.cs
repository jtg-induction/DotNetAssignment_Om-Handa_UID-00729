using DotNet_Assignment.Constants;
using DotNet_Assignment.Data;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Models.Enums;
using DotNet_Assignment.Repository.Address;
using DotNet_Assignment.Repository.Orders;
using DotNet_Assignment.Repository.Restaurants;
using DotNet_Assignment.Repository.Transaction;
using DotNet_Assignment.Repository.Users;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet_Assignment.Services.Orders
{
    public class OrderService :IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly AppDbContext _appDbContext;

        public OrderService(
            IOrderRepository orderRepository,
            IRestaurantRepository restaurantRepository,
            IAddressRepository addressRepository,
            IUserRepository userRepository,
            ITransactionRepository transactionRepository,
            AppDbContext appDbContext
            )
        {
            _orderRepository = orderRepository;
            _restaurantRepository = restaurantRepository;
            _addressRepository = addressRepository;
            _userRepository = userRepository;
            _appDbContext = appDbContext;
            _transactionRepository = transactionRepository;
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
            using (var transaction = _transactionRepository.BeginTransaction())
            {
                var user = await _userRepository.GetUserByIdAsync(userId);

                if (user == null)
                {
                    throw new Exception(ExceptionMessages.UserNotFound);
                }

                if (user.IsDeleted)
                {
                    throw new Exception(ExceptionMessages.UserDeactivated);
                }

                var restaurant = await _restaurantRepository.GetRestaurantByIdAsync(orderRequestDto.RestaurantId);

                if (restaurant == null)
                {
                    throw new Exception(ExceptionMessages.RestaurantNotFound);
                }

                if (!restaurant.MenuItems.Any())
                {
                    throw new Exception(ExceptionMessages.NoMenuItems);
                }

                if (orderRequestDto.OrderedItems == null)
                {
                    throw new Exception(ExceptionMessages.AtleastOneOrderItemRequired);
                }

                var address = await _addressRepository.GetAddressByIdAsync(orderRequestDto.AddressId, userId);

                string deliveryAddress = address.HouseNumber + ", " + address.Street + ", " + address.City + ", " + address.State + ", " + address.Pincode + ", " + address.Landmark;

                var order = new Order
                {
                    DeliveryAddress = deliveryAddress,
                    UserId = userId,
                    RestaurantId = orderRequestDto.RestaurantId
                };

                decimal totalPrice = 0m;

                foreach (OrderItemDto item in orderRequestDto.OrderedItems)
                {
                    var menuItem = await _restaurantRepository.GetMenuItemForUpdateAsync(item.MenuItemId);

                    if(menuItem.QuantityAvailable<0 || (menuItem.QuantityAvailable - item.Quantity) < 0)
                    {
                        throw new Exception(ExceptionMessages.InsufficientStock);
                    }

                    if (menuItem == null || menuItem.RestaurantId != orderRequestDto.RestaurantId)
                    {
                        throw new Exception(ExceptionMessages.MenuItemNotFound);
                    }

                    if ((user.Balance - (menuItem.Price * item.Quantity)) < 0)
                    {
                        throw new Exception(ExceptionMessages.InsufficientBalance);
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
                transaction.Commit();

                return new OrderResponseDto
                {
                    TotalPrice = order.TotalPrice,
                    OrderId = order.OrderId
                };
            }
        }

        /// <summary>
        /// Get order details of a specif order
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>Order Details</returns>
        /// <exception cref="Exception">if no order found</exception>
        public async Task<OrderDetailsResponseDto> GetOrderDetailsAsync(Guid orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);

            if(order == null)
            {
                throw new Exception(ExceptionMessages.OrderNotFound);
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
                    Price= item.ItemPrice,
                    Quantity= item.Quantity
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
            using (var transaction =_transactionRepository.BeginTransaction())
            {

                var order = await _orderRepository.GetOrderForUpdateAsync(orderId);

                if (order == null)
                {
                    throw new Exception(ExceptionMessages.OrderNotFound);
                }
                var orderUserId = order.UserId;

                if (userId != orderUserId)
                {
                    throw new Exception(ExceptionMessages.OrderCantBeCancelled);
                }

                var user = await _userRepository.GetUserByIdAsync(orderUserId);

                if (user == null)
                {
                    throw new Exception(ExceptionMessages.UserNotFound);
                }

                if (user.IsDeleted)
                {
                    throw new Exception(ExceptionMessages.UserDeactivated);
                }

                if (order.Status == OrderStatus.Cancelled)
                {
                    throw new Exception(ExceptionMessages.OrderAlreadyCancelled);
                }

                foreach (OrderedItem orderedItem in order.OrderedItems)
                {
                    var menuItem = await _restaurantRepository.GetMenuItemByIdAsync(orderedItem.OrderedItemId);

                    menuItem.QuantityAvailable += orderedItem.Quantity;
                }

                user.Balance += order.TotalPrice;
                order.Status = OrderStatus.Cancelled;
                order.UpdatedAt = DateTime.UtcNow;

                await _appDbContext.SaveChangesAsync();
                transaction.Commit();

            }
        }

        public async Task ChangeOrderStatusAsync(ChangeOrderStatusDto orderStatusDto)
        {
            var order = await _orderRepository.GetOrderForUpdateAsync(orderStatusDto.OrderId);

            if (order == null)
            {
                throw new Exception(ExceptionMessages.OrderNotFound);
            }

            if(orderStatusDto.Status == OrderStatus.Cancelled)
            {
                await CancelOrderAsync(order.OrderId, order.UserId);
                return;
            }

            order.Status = orderStatusDto.Status;

            await _appDbContext.SaveChangesAsync();
        }
    }
}
