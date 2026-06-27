using AuthService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Interfaces
{
    public interface IAddressService
    {
        Task<AddressDto> SaveAddress(string userId,CreateAddressDto dto);
        Task<AddressDto> UpdateAddress(string userId, UpdateAddressDto dto);
        Task DeleteAddress(string userId,int addressId);
        Task<IEnumerable<AddressDto>> GetAddresses(string userId);
        Task<AddressDto> GetDefaultAddress(string userId);
    }
}
