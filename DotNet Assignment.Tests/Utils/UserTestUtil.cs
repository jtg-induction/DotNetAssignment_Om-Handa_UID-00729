using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Models.Enums;
using Microsoft.Owin.BuilderProperties;
using System;
using System.IO;

namespace DotNet_Assignment.Tests.Utils
{
    public static class UserTestUtil
    {
        public static UpdateUserDto CreateMockUpdateUserDto()
        {
            return new UpdateUserDto
            {
                Name = "Updated User",
                PhoneNumber = "9870654321",
            };
        }

        public static User CreateMockUser()
        {
            return new User
            {
                UserId = Guid.NewGuid(),
                Name = "Om Handa",
                Email = "omhanda@test.com",
                Password = "password",
                PhoneNumber = "9870654321",
                Role= UserRoles.User,
                IsDeleted= false,
            };
        }

        public static AddressDto CreateMockAddressDto()
        {
            return new AddressDto
            {
                HouseNumber = "456",
                Street = "Updated Street",
                Landmark = "Updated Landmark",
                City = "New Delhi",
                State = "New Delhi",
                Pincode = "111111",
            };
        }

        public static UserAddress CreateMockAddress()
        {
            var user = CreateMockUser();
            return new UserAddress
            {
                HouseNumber = "123",
                Street = "Test Street",
                Landmark = "Landmark",
                City = "Delhi",
                State = "Delhi",
                Pincode = "110001",
                UserId = user.UserId,
                User = user
            };
        }
    }
}
