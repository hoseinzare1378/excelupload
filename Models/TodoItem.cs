using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace excelupload.Models;

public class TodoItem: BaseEntity
{
     public int ListId { get; set; }

    public string? Title { get; set; }

    public string? Note { get; set; }

    public PriorityLevel Priority { get; set; }

    public DateTime? Reminder { get; set; }

    

    public TodoList List { get; set; } = null!;
}
