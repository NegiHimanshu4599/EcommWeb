namespace NotificationService.Application.Dtos.DeviceToken
{
    public class DeviceTokenDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string Platform { get; set; } = null!;
        public bool IsActive { get; set; }
        public string DeviceId { get; set; } = null!;
    }
}
