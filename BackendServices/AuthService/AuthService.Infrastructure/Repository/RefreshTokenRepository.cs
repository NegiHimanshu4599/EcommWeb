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
    public class RefreshTokenRepository:Repository<RefreshToken> ,IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;
        public RefreshTokenRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<RefreshToken>> GetActiveTokensByUserId(string userId)
        {
            return await _context.RefreshTokens.Where(x => x.UserId == userId && 
            !x.IsRevoked && x.ExpiryDate > DateTime.UtcNow).ToListAsync();
        }

        public async Task RevokeAllUserTokens(string userId)
        {
           await _context.RefreshTokens.AsNoTracking().Where(x => x.UserId == userId
           && !x.IsRevoked && x.ExpiryDate > DateTime.UtcNow)
                .ExecuteUpdateAsync(x => x.SetProperty(p => p.IsRevoked, true));
        }
    }
}
