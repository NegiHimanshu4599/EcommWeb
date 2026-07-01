using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Dtos.DeviceToken
{
    public  class RegisterDeviceTokenDto
    {
        public string UserId { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string Platform { get; set; } = null!;
    }
}
