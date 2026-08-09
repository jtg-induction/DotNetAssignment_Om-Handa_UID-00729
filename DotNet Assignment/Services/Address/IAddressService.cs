using DotNet_Assignment.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet_Assignment.Services.Address
{
    public interface IAddressService
    {
        Task AddUserAddressAsync(Guid userId, AddressDto addressDto);
        
        Task UpdateUserAddressAsync(Guid userid, Guid userAddressId, AddressDto addressDto);
    }
}
