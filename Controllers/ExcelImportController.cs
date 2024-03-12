using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using excelupload.MediatRRequests.TodoItemBatchInsert.Commands;
using excelupload.Seed;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace excelupload.Controllers;

public class ExcelImportController: ControllerBase
{
    private IMediator mediator;
    public ExcelImportController(IMediator mediator)
    {
        this.mediator = mediator;
    }
    [HttpPost("upload")]
    public async Task<IActionResult> UploadExcelFile(IFormFile file)
    {
       var progressHubId = await mediator.Send(new TodoItemBatchInsertCommand(file));
       return Ok(new { Message = "Processing started", ProgressHubId = progressHubId });
    }


    [HttpPost("[action]")]
    public async Task<IActionResult> CreateTestExcelForMe()
    {
       ExcelSeeder.CreateSeedExcelFile("./TodoItemsSeed.xlsx");
       return Ok();
    }
}
