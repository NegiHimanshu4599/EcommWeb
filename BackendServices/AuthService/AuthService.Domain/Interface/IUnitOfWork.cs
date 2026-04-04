using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Interface
{
    public interface IUnitOfWork
    {
        public IRefreshTokenRepository RefreshToken { get; }
        public IUserRepository User { get; }
        Task SaveAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
