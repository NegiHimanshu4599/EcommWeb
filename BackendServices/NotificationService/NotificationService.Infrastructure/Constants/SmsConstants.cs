using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.Constants
{
    public static class SmsConstants
    {
        public const string SmsEndpoint ="/api/v5/flow/";
        public const string AuthHeader ="Authkey";
        public const string Json = "application/json";
    }
}
