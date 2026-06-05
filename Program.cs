using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IAIService, GeminiAIService>();

builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithTools<WeatherAnalysisTools>()
    .WithTools<OccupancyAnalysisTools>()
    .WithTools<ParkingAnalysisTools>();

var app = builder.Build();

app.MapMcp("/mcp");

await app.RunAsync();
