using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.DTOs
{
    public class ForgotPasswordDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
