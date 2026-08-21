using DotNet_Assignment.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet_Assignment.Repository.Address
{
    public interface IAddressRepository
    {
        Task<UserAddress> GetAddressByIdAsync(Guid userAddressId, Guid userId);

        void AddAddress(UserAddress userAddress);
    }
}
