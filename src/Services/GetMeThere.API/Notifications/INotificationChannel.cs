using GetMeThere.API.Models;

namespace GetMeThere.API.Notifications
{
    public record NotificationPayload(string EventName, object Body);

    public interface INotificationChannel
    {
        string Name { get; }
        int Priority { get; }
        Task<bool> TrySendAsync(User user, NotificationPayload payload, CancellationToken ct = default);
    }

    public interface INotificationDispatcher
    {
        Task<bool> SendAsync(User user, NotificationPayload payload, CancellationToken ct = default);
    }
}
