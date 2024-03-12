using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using excelupload.hubs;
using excelupload.Models;
using Ganss.Excel;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace excelupload.MediatRRequests.TodoItemBatchInsert.Commands;

public class TodoItemBatchInsertCommand : IRequest<string>
{
    public IFormFile ExcelFile { get; set; }
    public string ConnectionId { get; set; }

    public TodoItemBatchInsertCommand(IFormFile file)
    {
        ExcelFile = file;
    }
}


public class TodoItemBatchInsertCommandHandler : IRequestHandler<TodoItemBatchInsertCommand, string>
{
    private readonly IConfiguration _configuration;
    private readonly IHubContext<ProgressHub> _progressHub;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public TodoItemBatchInsertCommandHandler(IConfiguration configuration, IHubContext<ProgressHub> progressHub, IBackgroundJobClient backgroundJobClient)
    {
        _configuration = configuration;
        _progressHub = progressHub;
        _backgroundJobClient = backgroundJobClient;
    }
    public async Task<string> Handle(TodoItemBatchInsertCommand request, CancellationToken cancellationToken)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        var tempFileName = Path.GetTempFileName();
        await using (var fileStream = new FileStream(tempFileName, FileMode.Create, FileAccess.Write))
        {
            await request.ExcelFile.CopyToAsync(fileStream);
            await fileStream.FlushAsync();
        }

        var items = new List<TodoItem>();

        try
        {
            var mapper = new ExcelMapper(tempFileName);
            var itemsDtos = mapper.Fetch<TodoItemDto>().ToList();

            items = itemsDtos.Select(i => new TodoItem
            {
                ListId = i.ListId,
                Title = i.Title,
                Note = i.Note,
                Priority = (PriorityLevel)Convert.ToInt32(i.Priority),
                Reminder = i.Reminder
            }).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while reading the Excel file: {ex.Message}");
        }
        var chunkSize = 100;
        var taskId = Guid.NewGuid().ToString();

        var chunks = items.Chunk(chunkSize).ToList();
        string previousJobId = null;
        for (int i = 0; i < chunks.Count(); i++)
        {
            if (string.IsNullOrEmpty(previousJobId))
            {
                previousJobId = _backgroundJobClient.Enqueue(() => ExcelFileProcessor.ProcessChunk(chunks[i], connectionString, taskId, items.Count, i, chunks.Count));
            }
            else
            {
                previousJobId = _backgroundJobClient.ContinueJobWith(previousJobId, () => ExcelFileProcessor.ProcessChunk(chunks[i], connectionString, taskId, items.Count, i, chunks.Count));
            }
        }

        await _progressHub.Clients.All.SendAsync("ReceiveProgress", taskId, 0);

        File.Delete(tempFileName);

        return taskId;
    }

}
