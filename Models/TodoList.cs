using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace excelupload.Models;

public class TodoList: BaseEntity
{
    public string? Title { get; set; }
    public IList<TodoItem> Items { get; private set; } = new List<TodoItem>();
}
