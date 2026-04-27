using GetMeThere.API.Models;

namespace GetMeThere.API.Notifications
{
    public class NotificationDispatcher : INotificationDispatcher
    {
        private readonly IReadOnlyList<INotificationChannel> _channels;
        private readonly ILogger<NotificationDispatcher> _logger;

        public NotificationDispatcher(IEnumerable<INotificationChannel> channels, ILogger<NotificationDispatcher> logger)
        {
            _channels = channels.OrderBy(c => c.Priority).ToList();
            _logger = logger;
        }

        public async Task<bool> SendAsync(User user, NotificationPayload payload, CancellationToken ct = default)
        {
            foreach (var channel in _channels)
            {
                try
                {
                    if (await channel.TrySendAsync(user, payload, ct))
                        return true;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Channel {Channel} failed for user {UserId} event {Event}",
                        channel.Name, user.Id, payload.EventName);
                }
            }
            _logger.LogInformation("No channel delivered event {Event} to user {UserId}", payload.EventName, user.Id);
            return false;
        }
    }
}
