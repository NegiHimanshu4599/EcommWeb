namespace NotificationService.Application.Dtos.DeviceToken
{
    public  class RegisterDeviceTokenDto
    {
        public string UserId { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string Platform { get; set; } = null!;
        public string DeviceId { get; set; } = null!;
    }
}
