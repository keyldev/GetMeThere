using GetMeThere.API.Hubs;
using GetMeThere.API.Models;
using GetMeThere.API.Services;
using Microsoft.AspNetCore.SignalR;

namespace GetMeThere.API.Notifications
{
    public class SignalRChannel : INotificationChannel
    {
        private readonly IHubContext<TaxiHub> _hub;

        public SignalRChannel(IHubContext<TaxiHub> hub)
        {
            _hub = hub;
        }

        public string Name => "signalr";
        public int Priority => 0;

        public async Task<bool> TrySendAsync(User user, NotificationPayload payload, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(user.ConnectionId))
                return false;

            await _hub.Clients.Group(Groups.Client(user.Id))
                .SendAsync(payload.EventName, payload.Body, cancellationToken: ct);
            return true;
        }
    }
}
