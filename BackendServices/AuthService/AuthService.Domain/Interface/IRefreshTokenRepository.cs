using AuthService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Interface
{
    public interface IRefreshTokenRepository: IRepository<RefreshToken, int>
    {
        Task<IEnumerable<RefreshToken>> GetActiveTokensByUserId(string userId);
        Task RevokeAllUserTokens(string userId);
        Task<RefreshToken?> GetByTokenAsync(string hashedToken);
    }
}