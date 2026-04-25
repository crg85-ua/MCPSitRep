using System.Text.Json;

/// <summary>
/// Clase base compartida por todas las herramientas MCP de análisis.
/// Centraliza la configuración común: cliente HTTP, servicio de IA, URL base y serialización JSON.
/// </summary>
internal abstract class AnalysisToolBase
{
    protected readonly HttpClient _httpClient;
    protected readonly IAIService _aiService;

    protected const string BaseApiUrl = "https://localhost:44384/GetRawData";

    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    protected AnalysisToolBase(HttpClient httpClient, IAIService aiService)
    {
        _httpClient = httpClient;
        _aiService = aiService;
    }

    protected static string SerializeSuccess(object result) =>
        JsonSerializer.Serialize(result, JsonOptions);

    protected static string SerializeError(string idEspacio, string error, string message) =>
        JsonSerializer.Serialize(new
        {
            success = false,
            timestamp = DateTime.UtcNow,
            idEspacio,
            error,
            message
        }, JsonOptions);
}
