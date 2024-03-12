using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace excelupload.MediatRRequests.TodoItemBatchInsert.Commands;

public class TodoItemDto
{
    public int ListId { get; set; }
    public string Title { get; set; }
    public string Note { get; set; }
    public double Priority { get; set; }
    public DateTime? Reminder { get; set; }
}
