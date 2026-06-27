using AuthService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Interface
{
    public interface IAddressRepository: IRepository<Address, int>
    {
        Task<IEnumerable<Address>> GetByUserIdAsync(string userId);
        Task<Address?> GetDefaultAddressAsync(string userId);
        Task<Address?> GetByIdAndUserIdAsync(int addressId,string userId);
    }
}