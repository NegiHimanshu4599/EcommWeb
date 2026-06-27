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
    public class UserProfileRepository:GenericRepository<UserProfile,string> , IUserProfileRepository
    {
        public UserProfileRepository( ApplicationDbContext context) : base(context)
        {
        }
        public async Task<UserProfile?> GetByUserIdAsync(string userId)
        {
            return await _context.UserProfiles.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId);
        }
    }
}
