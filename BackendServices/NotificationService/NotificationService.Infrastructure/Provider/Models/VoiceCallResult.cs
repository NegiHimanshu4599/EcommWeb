namespace NotificationService.Infrastructure.Provider.Models
{
    public sealed class VoiceCallResult
    {
        public bool Success { get; init; }
        public string? CallSid { get; init; }
        public string? Status { get; init; }
        public string? ErrorMessage { get; init; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public string RecordingUri { get; set; }
        public string Duration { get; set; }
        public double Price { get; set; }
        public string Uri { get; set; }
    }
}