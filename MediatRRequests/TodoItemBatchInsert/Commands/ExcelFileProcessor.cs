using Dapper;
using Microsoft.AspNetCore.SignalR;
using excelupload.hubs;
using excelupload.Models;
using excelupload.MediatRRequests.TodoItemBatchInsert;
using Microsoft.Data.SqlClient;

public class ExcelFileProcessor
{
    private readonly IHubContext<ProgressHub> _progressHub;
    private readonly string _connectionString;

    public ExcelFileProcessor(IHubContext<ProgressHub> progressHub, string connectionString)
    {
        _progressHub = progressHub;
        _connectionString = connectionString;
    }

    public static async Task ProcessChunk(IEnumerable<TodoItem> items, string connectionString, string taskId, int totalItems, int currentChunkNumber, int totalChunks)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        var sql = @"INSERT INTO TodoItems (ListId, Title, Note, Priority, Reminder) 
                VALUES (@ListId, @Title, @Note, @Priority, @Reminder)";

        await connection.ExecuteAsync(sql, items);

        var progressPercentage = (int)((double)(currentChunkNumber + 1) / totalChunks * 100);
        ProgressTracker.UpdateProgress(taskId, progressPercentage);
    }
}
