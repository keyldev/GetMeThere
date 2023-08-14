using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR; // always use this AspNetCore

using System.Diagnostics;

namespace GetMeThere.API.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TaxiHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var username = Context.User.Identity.Name;
            Debug.WriteLine("Name is " + username + " " + Context.ConnectionId);
            await base.OnConnectedAsync();
        }
    }
}
