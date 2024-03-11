using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using excelupload.Models;
using Ganss.Excel;
using MediatR;

namespace excelupload.MediatRRequests.TodoItemBatchInsert.Commands;

public class TodoItemBatchInsertCommand: IRequest<Unit>
{
    public IFormFile ExcelFile { get; set; }
}


public class TodoItemBatchInsertCommandHandler : IRequestHandler<TodoItemBatchInsertCommand, Unit>
{
    public Task<Unit> Handle(TodoItemBatchInsertCommand request, CancellationToken cancellationToken)
    {
            using var stream = request.ExcelFile.OpenReadStream();
            var todoItems = new ExcelMapper(stream).Fetch<TodoItem>()

            // Divide and enqueue messages
            var chunkSize = 1000;
            var todoItemChunks = todoItems.Chunk(chunkSize);

            foreach (var chunk in todoItemChunks)
            {
                await _mediator.Send(new ProcessTodoItemsCommand(chunk), cancellationToken);
            }

            return Unit.Value;
    }
}
