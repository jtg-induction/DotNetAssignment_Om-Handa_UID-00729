using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNet_Assignment.Constants
{
    public class ExceptionMessages
    {
        public const string InvalidCredentials = "Invalid credentials";
        public const string UserNotFound = "User not found";
        public const string RefreshTokenNotGenerated = "Refresh token could not be generated";
        public const string EmailAlreadyExists = "Email already exists";
        public const string UserDeactivated = "User is deactivated";
        public const string UserAlreadyDeactivated = "User already deactivated";
        public const string AddressNotFound = "Address not found";
        public const string RefreshTokenNotFound = "Refresh token not found";
        public const string RefreshTokenInvalid = "Refresh token invalid";
        public const string RefreshTokenExpired = "Refresh token Expired";
        public const string OldPasswordIncorrect = "Old Password is Incorrect";
        public const string OneFieldRequired = "Atleast one field is required";
        public const string RestaurantNotFound = "Restaurant not Found";
        public const string NoMenuItems = "This restaurant has no menu items";
        public const string NoRestaurants = "No Restaurants found";
        public const string AtleastOneOrderItemRequired = "Atleast one order item required";
        public const string MenuItemNotFound = "Menu Item not found";
        public const string InsufficientBalance = "Insufficient Balance";
        public const string OrderNotFound = "Order Not Found";
        public const string OrderAlreadyCancelled = "Order Already Cancelled";
        public const string OrderCantBeCancelled = "Order Can't be Cancelled";
        public const string InsufficientStock = "Insufficient Stock";
        public const string StatusCantBeChanged = "Order status can't be changed";
        public const string NoOrdersToShow = "No Orders To Show";
        public const string Unauthorized = "Unauthorized";
        public const string OrderStatusAlreadyChanged = "Order Status Already Changed";
        public const string InvalidOrderStatus = "Invalid Order Status";
        public const string OrderRejectedByRestaurant = "Order Rejected By Restaurant";

    }
}
