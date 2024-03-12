using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace excelupload.hubs
{
    public class ProgressHub : Hub
    {
        public async Task SendProgress(string progressHubId, string message)
        {
            await Clients.Group(progressHubId).SendAsync("ReceiveProgress", message);
        }

        public override async Task OnConnectedAsync()
        {
            var progressHubId = Context.GetHttpContext().Request.Query["progressHubId"];
            await Groups.AddToGroupAsync(Context.ConnectionId, progressHubId);
            await base.OnConnectedAsync();
        }
    }
}