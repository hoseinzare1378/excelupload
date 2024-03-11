using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using excelupload.Models;
using Ganss.Excel;
using MassTransit;
using MediatR;

namespace excelupload.MediatRRequests.TodoItemBatchInsert.Commands;

public class TodoItemBatchInsertCommand: IRequest<Unit>
{
    public IFormFile ExcelFile { get; set; }
}


public class TodoItemBatchInsertCommandHandler : IRequestHandler<TodoItemBatchInsertCommand, Unit>
{
    private readonly IPublishEndpoint _publishEndpoint;
    public TodoItemBatchInsertCommandHandler(IPublishEndpoint publishEndPoint)
    {
        _publishEndpoint = publishEndPoint;
    }
    public async Task<Unit> Handle(TodoItemBatchInsertCommand request, CancellationToken cancellationToken)
    {
            using var stream = request.ExcelFile.OpenReadStream();
            var todoItems = new ExcelMapper(stream).Fetch<TodoItem>();

            // Divide and enqueue messages
            var chunkSize = 1000;
            var todoItemChunks = todoItems.Chunk(chunkSize);
            var processedChunks = 0;

             foreach (var chunk in todoItemChunks)
             {
                 await _publishEndpoint.Publish(new ProcessTodoItemsMessage(chunk), cancellationToken);
                 processedChunks++;
             }

            return Unit.Value;
    }
}
