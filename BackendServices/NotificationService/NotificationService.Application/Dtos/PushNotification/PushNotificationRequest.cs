namespace NotificationService.Application.Dtos.PushNotification
{
    public sealed class PushNotificationRequest
    {
        public string DeviceToken { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string? ClickAction { get; set; }
        public Dictionary<string, string>? Data { get; set; }
        public bool HighPriority { get; set; } = true;
    }
}