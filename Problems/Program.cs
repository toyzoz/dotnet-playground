using Microsoft.AspNetCore.Http.Features;
using Problems;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        var httpContext = context.HttpContext;
        context.ProblemDetails.Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}";
        context.ProblemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        var activity = httpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        context.ProblemDetails.Extensions["requestId"] = activity?.Id;
        context.ProblemDetails.Extensions["appName"] = "problems";
        // Add more details to the problem details
    };
});
// Register the ProblemExceptionHandler
builder.Services.AddExceptionHandler<ProblemExceptionHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();


        return forecast;
    })
    .WithName("GetWeatherForecast");


app.MapGet("/weatherforecast/error", () => Results.Problem(
        title: "An error occurred while processing your request.",
        detail: "An error occurred while processing your request.",
        statusCode: StatusCodes.Status400BadRequest)
    )
    .WithName("error");

app.MapGet("/weatherforecast/error2", () =>
    {
        throw new ProblemException(
            "An error occurred while processing your request.",
            "An error occurred while processing your request.");
    })
    .WithName("error2");


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}