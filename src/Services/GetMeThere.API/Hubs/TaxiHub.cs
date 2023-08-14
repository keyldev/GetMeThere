using GetMeThere.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR; // always use this AspNetCore

using System.Diagnostics;

namespace GetMeThere.API.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TaxiHub : Hub
    {
        private readonly ITaxiHubService _taxiHubService;

        public TaxiHub(ITaxiHubService taxiHubService)
        {

            _taxiHubService = taxiHubService;

        }

        public override async Task OnConnectedAsync()
        {
            var username = Context.User.Identity.Name;
            var connectionId = Context.ConnectionId;
            
            await _taxiHubService.UpdateUserConnectionId(username, connectionId);

            await base.OnConnectedAsync();
        }
    }
}
