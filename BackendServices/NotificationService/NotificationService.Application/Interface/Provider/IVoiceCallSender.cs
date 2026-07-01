namespace NotificationService.Application.Interface.Provider
{
    public interface IVoiceCallSender
    {
        Task SendVoiceMessageAsync(string phoneNumber, string message);
        Task SendOtpCallAsync(string phoneNumber, string otp);
    }
}