using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace excelupload.Models;

public class BaseEntity
{
    public int Id { get; set; }
    public DateTimeOffset Created { get; set; }

    public string? CreatedBy { get; set; }

    public DateTimeOffset LastModified { get; set; }

    public string? LastModifiedBy { get; set; }
}
