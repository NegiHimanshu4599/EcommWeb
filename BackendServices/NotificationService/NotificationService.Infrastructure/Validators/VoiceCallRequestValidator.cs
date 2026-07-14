using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.Validators
{
    internal static class VoiceCallRequestValidator
    {
        public static void Validate(
            string phoneNumber,
            string message)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(phoneNumber);
            ArgumentException.ThrowIfNullOrWhiteSpace(message);
            if (message.Length > 500)
                throw new ArgumentException("Voice message exceeds maximum allowed length.",nameof(message));
        }
    }
}
