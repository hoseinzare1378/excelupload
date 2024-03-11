using excelupload.hubs;
using excelupload.MediatRRequests.TodoItemBatchInsert.Commands;
using MassTransit.Mediator;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddMediatR(typeof(TodoItemBatchInsertCommand).Namespace);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapPost("/import", async (IFormFile file, string connectionId, IMediator mediator) =>
{
    if (file.Length <= 0)
    {
        return Results.BadRequest("Empty file");
    }

    // Invoke the MediatR request to process the file
    string result = await mediator.Send(new TodoItemBatchInsertCommand(file, connectionId));

    return Results.Ok(result);
}).Accepts<IFormFile>("multipart/form-data");
.WithName("GetWeatherForecast")
.WithOpenApi();


app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ProgressHub>("/progressHub");
});



app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
