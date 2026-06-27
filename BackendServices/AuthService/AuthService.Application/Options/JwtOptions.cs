using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Options
{
    public sealed class JwtOptions
    {
        public const string SectionName = "Jwt";
        public string Key { get; init; } = null!;
        public string Issuer { get; init; } = null!;
        public string Audience { get; init; } = null!;
        public int DurationInMinutes { get; init; }
    }
}
