using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IAIService, GeminiAIService>();
builder.Services.AddScoped<WeatherAnalysisTools>(sp =>
    new WeatherAnalysisTools(
        sp.GetRequiredService<IHttpClientFactory>().CreateClient(),
        sp.GetRequiredService<IAIService>()));
builder.Services.AddScoped<OccupancyAnalysisTools>(sp =>
    new OccupancyAnalysisTools(
        sp.GetRequiredService<IHttpClientFactory>().CreateClient(),
        sp.GetRequiredService<IAIService>()));
builder.Services.AddScoped<ParkingAnalysisTools>(sp =>
    new ParkingAnalysisTools(
        sp.GetRequiredService<IHttpClientFactory>().CreateClient(),
        sp.GetRequiredService<IAIService>()));

builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithTools<WeatherAnalysisTools>()
    .WithTools<OccupancyAnalysisTools>()
    .WithTools<ParkingAnalysisTools>();

var app = builder.Build();

app.MapMcp("/mcp");

// ===== WEATHER ANALYSIS ENDPOINT =====
app.MapPost("/api/analysis/weather", async (
    WeatherAnalysisTools tool,
    ILogger<Program> logger,
    WeatherAnalysisRequest request) =>
{
    logger.LogInformation("Weather analysis requested for: {IdEspacio}", request.IdEspacio);
    var result = await tool.GetWeatherAnalysisWithAI(
        request.IdEspacio,
        request.IncludeSafetyRecommendations,
        request.IncludeTrendAnalysis);
    return Results.Content(result, "application/json");
}).WithName("AnalyzeWeather");

// ===== OCCUPANCY ANALYSIS ENDPOINT =====
app.MapPost("/api/analysis/occupancy", async (
    OccupancyAnalysisTools tool,
    ILogger<Program> logger,
    OccupancyAnalysisRequest request) =>
{
    logger.LogInformation("Occupancy analysis requested for: {IdEspacio}", request.IdEspacio);
    var result = await tool.GetOccupancyAnalysisWithAI(
        request.IdEspacio,
        request.IncludeAlerts);
    return Results.Content(result, "application/json");
}).WithName("AnalyzeOccupancy");

// ===== PARKING ANALYSIS ENDPOINT =====
app.MapPost("/api/analysis/parking", async (
    ParkingAnalysisTools tool,
    ILogger<Program> logger,
    ParkingAnalysisRequest request) =>
{
    logger.LogInformation("Parking analysis requested for: {IdEspacio}", request.IdEspacio);
    var result = await tool.GetParkingAnalysisWithAI(
        request.IdEspacio,
        request.Fecha);
    return Results.Content(result, "application/json");
}).WithName("AnalyzeParking");

// ===== MCP TOOLS INFO ENDPOINT =====
app.MapGet("/api/tools/info", () =>
{
    return Results.Ok(new
    {
        status = "available",
        timestamp = DateTime.UtcNow,
        tools = new object[]
        {
            new
            {
                name = "WeatherAnalysisTools",
                description = "Analyzes meteorological data and provides weather insights",
                methods = new[] { "GetWeatherAnalysisWithAI" }
            },
            new
            {
                name = "OccupancyAnalysisTools",
                description = "Analyzes occupancy data for spaces",
                methods = new[] { "GetOccupancyAnalysisWithAI" }
            },
            new
            {
                name = "ParkingAnalysisTools",
                description = "Analyzes parking space availability and management",
                methods = new[] { "GetParkingAnalysisWithAI" }
            }
        },
        mcp = new
        {
            protocol = "HTTP Transport",
            endpoint = "/mcp"
        }
    });
}).WithName("GetToolsInfo");


await app.RunAsync();
