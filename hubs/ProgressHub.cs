using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace excelupload.hubs
{
    public class ProgressHub : Hub
{
    public async Task UpdateProgress(string connectionId, string message)
    {
        await Clients.Client(connectionId).SendAsync("ReceiveProgress", message);
    }
}
}