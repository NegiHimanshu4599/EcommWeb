using AuthService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfileDto> GetProfile(string userId);
        Task<UserProfileDto> UpdateProfile( string userId, UpdateUserProfileDto dto);
    }
}
