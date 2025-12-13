using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

// Configure all logs to go to stderr (stdout is used for the MCP protocol messages).
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

// Add HttpClient for temperature sensor API calls
builder.Services.AddHttpClient();

// Add AI Service for real Llama integration using OllamaSharp
builder.Services.AddSingleton<IAIService, LlamaAIService>();

// Add the MCP services: the transport to use (stdio) and the tools to register.
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<RandomNumberTools>()
    .WithTools<TemperatureSensorTools>()
    .WithTools<OccupancyAnalysisTools>()
    .WithTools<ParkingAnalysisTools>();

await builder.Build().RunAsync();
