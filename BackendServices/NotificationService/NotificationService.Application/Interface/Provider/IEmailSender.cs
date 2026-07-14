namespace NotificationService.Application.Interface.Provider
{
    public interface IEmailSender
    {
        Task SendAsync(string to, string subject, string body);
    }
}
