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
    public class RefreshTokenRepository:GenericRepository<RefreshToken,int> ,IRefreshTokenRepository
    {
        public RefreshTokenRepository(ApplicationDbContext context):base(context)
        {
        }
        public async Task<IEnumerable<RefreshToken>> GetActiveTokensByUserId(string userId)
        {
            return await _context.RefreshTokens.Where(x => x.UserId == userId && 
            !x.IsRevoked && x.ExpiryDate > DateTime.UtcNow).ToListAsync();
        }
        public async Task<RefreshToken?> GetByTokenAsync(string hashedToken)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == hashedToken);
        }
        public async Task RevokeAllUserTokens(string userId)
        {
           await _context.RefreshTokens.AsNoTracking()
                .Where(x => x.UserId == userId && !x.IsRevoked && x.ExpiryDate > DateTime.UtcNow)
                .ExecuteUpdateAsync(x => x.SetProperty(p => p.IsRevoked, true)
                .SetProperty(p => p.RevokedAt, DateTime.UtcNow));
        }
    }
}
