namespace NotificationService.Application.Interface.Provider
{
    public interface IVoiceCallSender
    {
        Task SendVoiceMessageAsync( string phoneNumber, string message,CancellationToken cancellationToken = default);
        Task SendOtpCallAsync( string phoneNumber,string otp,CancellationToken cancellationToken = default);
    }
}