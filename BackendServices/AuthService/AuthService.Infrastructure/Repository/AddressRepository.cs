using AuthService.Domain.Entities;
using AuthService.Domain.Interface;
using AuthService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Repository
{
    public class AddressRepository : GenericRepository<Address,int>, IAddressRepository
    {
        public AddressRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<Address?> GetByIdAndUserIdAsync(int addressId, string userId)
        {
            return await _context.Addresses.FirstOrDefaultAsync(x => x.Id == addressId && x.UserId == userId);
        }
        public async Task<IEnumerable<Address>> GetByUserIdAsync(string userId)
        {
            return await _context.Addresses.AsNoTracking().Where(x => x.UserId == userId).ToListAsync();
        }
        public async Task<Address?> GetDefaultAddressAsync(string userId)
        {
            return await _context.Addresses.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId && x.IsDefault);
        }
    }
}
    