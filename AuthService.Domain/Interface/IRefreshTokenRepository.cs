using AuthService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Interface
{
    public  interface IRefreshTokenRepository:IRepository<RefreshToken>
    {
        Task<IEnumerable<RefreshToken>> GetActiveTokensByUserId(string userId);
    }
}
