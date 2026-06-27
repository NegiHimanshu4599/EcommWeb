using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Entities
{
    public class UserProfile
    {
        public string UserId { get; set; } = null!;
        public string FullName { get; set; } = null!;
        // User's primary contact number
        public string PrimaryPhoneNumber { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ApplicationUser User { get; set; } = null!;
    }
}