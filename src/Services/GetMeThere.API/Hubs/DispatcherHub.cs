﻿using Microsoft.AspNetCore.SignalR;

namespace GetMeThere.API.Hubs
{
    public class DispatcherHub : Hub
    {

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

    }
}
