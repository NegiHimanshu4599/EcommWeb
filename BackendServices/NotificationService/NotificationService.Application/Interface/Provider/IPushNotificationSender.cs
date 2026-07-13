using NotificationService.Application.Dtos.PushNotification;

namespace NotificationService.Application.Interface.Provider
{
    public interface IPushNotificationSender
    {
        Task SendAsync(PushNotificationRequest request, CancellationToken cancellationToken = default);
        Task SendMulticastAsync(MulticastPushNotificationRequest request, CancellationToken cancellationToken = default);
    }
}