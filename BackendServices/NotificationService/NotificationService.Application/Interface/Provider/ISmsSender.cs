namespace NotificationService.Application.Interface.Provider
{
    public interface ISmsSender
    {
        Task SendAsync(string phoneNumber, string message,CancellationToken cancellationToken = default);
    }
}
