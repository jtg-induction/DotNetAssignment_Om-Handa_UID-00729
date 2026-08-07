using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Entities;
using System;
using System.Threading.Tasks;

namespace DotNet_Assignment.Repository.Users
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<bool> FindUserByEmailAsync(string email);

        Task<User> GetUserByIdAsync(Guid userId);

        Task<UserAddress> GetAddressByIdAsync(Guid userAddressId,Guid userId);

        void AddUser(User user);

        void AddAddress(UserAddress userAddress);

    }

}
