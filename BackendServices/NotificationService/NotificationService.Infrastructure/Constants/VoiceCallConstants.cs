using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.Constants
{
    public static class VoiceCallConstants
    {
        public const string CallsEndpoint = "/v1/Accounts/{0}/Calls/connect.json";
        public const string Json = "application/json";
        public const string BasicAuthentication = "Basic";
    }
}