namespace NotificationService.Infrastructure.Provider.Helpers
{
    internal static class PhoneNumberHelper
    {
        public static string Normalize(string phoneNumber, string countryCode)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number is required.",nameof(phoneNumber));
            phoneNumber = phoneNumber.Trim();
            if (phoneNumber.StartsWith("+"))
                phoneNumber = phoneNumber[1..];
            if (phoneNumber.StartsWith(countryCode))
                return phoneNumber;
            return $"{countryCode}{phoneNumber}";
        }
    }
}