using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using excelupload.hubs;
using Microsoft.AspNetCore.SignalR;

namespace excelupload.MediatRRequests.TodoItemBatchInsert;

public static class ProgressTracker
{
    private static readonly Dictionary<string, int> _progress = new();

    public static async void UpdateProgress(string taskId, int progress)
    {
        _progress[taskId] = progress;
        var hubContext = AppServices.ServiceProvider.GetService<IHubContext<ProgressHub>>();
        await hubContext.Clients.All.SendAsync("ReceiveMessage", progress + "%");

    }

    public static int GetProgress(string taskId)
    {
        return _progress.TryGetValue(taskId, out var progress) ? progress : 0;
    }
}

