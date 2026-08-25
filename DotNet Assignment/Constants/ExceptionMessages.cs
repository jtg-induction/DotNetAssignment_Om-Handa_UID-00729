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
        public const string OneFieldRequired = "At least one field is required";
        public const string RestaurantNotFound = "Restaurant not found";
        public const string NoMenuItems = "This restaurant has no menu items";
        public const string NoRestaurants = "No Restaurants found";
        public const string AtleastOneOrderItemRequired = "At least one order item required";
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
        public const string RestaurantIsClosed = "Restaurant Is Closed";
        public const string InvalidOperation = "Invalid Operation";
        public const string NewPasswordCantBeSameAsOld = "New password can't be same as old password";
        public const string UserALreadyAssignedToThisRestaurant = "User is already assigned to this restaurant";
        public const string PhoneNumberExists = "Phone Number Already Exists";
        public const string WeakPassword = "At least 8 characters, including one uppercase letter, one lowercase letter, one number, and one special character is required";
        public const string InvalidEmail = "Entered Email is not a valid Email";
        public const string InvalidPhoneNumber = "Entered phone number is not a valid phone number";
        public const string InvalidPincode = "Entered pincode is not a valid pincode";
        public const string OrderCancelledByUser = "Order Cancelled By User";
        public const string CantCancelOrderDelivered = "Can't cancel, Order has already been delivered";
        public const string CantCancelOrderDispatched = "Can't cancel, Order has already been dispatched";
        public const string OwnerNotAssignedToRestaurant = "Owner is not associated with the restauarant";
    }
}
