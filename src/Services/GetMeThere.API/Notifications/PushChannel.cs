using GetMeThere.API.Models;

namespace GetMeThere.API.Notifications
{
    // FCM integration lands in Phase 4. Until then this channel is a no-op so the
    // dispatcher can iterate past it and the abstraction stays in production code paths.
    public class PushChannel : INotificationChannel
    {
        private readonly ILogger<PushChannel> _logger;

        public PushChannel(ILogger<PushChannel> logger)
        {
            _logger = logger;
        }

        public string Name => "push";
        public int Priority => 10;

        public Task<bool> TrySendAsync(User user, NotificationPayload payload, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(user.FcmDeviceToken))
                return Task.FromResult(false);

            _logger.LogInformation("FCM send pending (Phase 4) for user {UserId} event {Event}",
                user.Id, payload.EventName);
            return Task.FromResult(false);
        }
    }
}
