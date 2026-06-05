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
        "devolviendo análisis con porcentaje de ocupación y recomendaciones operativas.")]
    public async Task<string> GetOccupancyAnalysisWithAI(
        [Description("ID del espacio de aforo a consultar")] string idEspacio,
        [Description("Incluir alertas y recomendaciones operativas")] bool includeAlerts = true)
    {
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
