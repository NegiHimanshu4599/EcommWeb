namespace NotificationService.Infrastructure.Provider.Helpers
{
    internal static class VoiceMessageBuilder
    {
        public static string BuildOtpMessage(string otp)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(otp);
            return $"Your verification code is {otp}. I repeat, your verification code is {otp}. This code will expire in ten minutes.";
        }
        public static string BuildCustomMessage(string message)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(message);
            return message.Trim();
        }
    }
}