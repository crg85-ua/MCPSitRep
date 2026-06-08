using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using Occupancy;

internal class OccupancyAnalysisTools : AnalysisToolBase
{
    public OccupancyAnalysisTools(HttpClient httpClient, IAIService aiService)
        : base(httpClient, aiService) { }

    [McpServerTool]
    [Description("Llama al endpoint de API de sensores de aforo usando el IdEspacio " +
        "y usa IA (Gemini) para interpretar los datos de ocupación del espacio, " +
        "devolviendo análisis con porcentaje de ocupación y recomendaciones operativas. " +
        "Acepta tanto el identificador canónico (ej: PlayaArenal_1) como el nombre en lenguaje natural (ej: Playa del Arenal).")]
    public async Task<string> GetOccupancyAnalysisWithAI(
        [Description("ID o nombre del espacio de aforo a consultar (ej: PlayaArenal_1, Playa del Arenal, PlayaGrava_2, PuertoJavea_3)")] string idEspacio,
        [Description("Incluir alertas y recomendaciones operativas")] bool includeAlerts = true)
    {
        idEspacio = ResolveIdEspacio(idEspacio);
        try
        {
            var apiUrl = $"{BaseApiUrl}/GetSpaceOccupancyDataBySpace/{idEspacio}";
            var response = await _httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            var rawData = await response.Content.ReadAsStringAsync();
            var sensorData = JsonSerializer.Deserialize<OccupancySensorData>(rawData, JsonOptions);
            var analysis = await _aiService.AnalyzeOccupancyDataAsync(rawData, sensorData, includeAlerts);

            return SerializeSuccess(new
            {
                success = true,
                timestamp = DateTime.UtcNow,
                idEspacio,
                dataType = "OCCUPANCY_SPACE_DATA",
                aiAnalysis = analysis
            });
        }
        catch (HttpRequestException ex)
        {
            return SerializeError(idEspacio, "API_CONNECTION_ERROR", $"Error al llamar a la API de aforo: {ex.Message}");
        }
        catch (Exception ex)
        {
            return SerializeError(idEspacio, "UNEXPECTED_ERROR", $"Error inesperado: {ex.Message}");
        }
    }

}
