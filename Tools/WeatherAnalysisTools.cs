using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;

internal class WeatherAnalysisTools : AnalysisToolBase
{
    public WeatherAnalysisTools(HttpClient httpClient, IAIService aiService)
        : base(httpClient, aiService) { }

    [McpServerTool]
    [Description("Llama al endpoint de API meteorológica usando el IdEspacio y usa IA (Gemini) para interpretar los datos del clima del área, " +
        "devolviendo un análisis estructurado en JSON.")]
    public async Task<string> GetWeatherAnalysisWithAI(
        [Description("ID del espacio meteorológico a consultar")] string idEspacio,
        [Description("Incluir recomendaciones de seguridad")] bool includeSafetyRecommendations = true,
        [Description("Incluir análisis de tendencias")] bool includeTrendAnalysis = true)
    {
        try
        {
            var apiUrl = $"{BaseApiUrl}/GetWeatherDataBySpace/{idEspacio}";
            var response = await _httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            var rawData = await response.Content.ReadAsStringAsync();
            var sensorData = JsonSerializer.Deserialize<TemperatureSensorData>(rawData, JsonOptions);
            var analysis = await _aiService.AnalyzeTemperatureDataAsync(rawData, sensorData, includeSafetyRecommendations, includeTrendAnalysis);

            return SerializeSuccess(new
            {
                success = true,
                timestamp = DateTime.UtcNow,
                idEspacio,
                dataType = "WEATHER_SPACE_DATA",
                aiAnalysis = analysis
            });
        }
        catch (HttpRequestException ex)
        {
            return SerializeError(idEspacio, "API_CONNECTION_ERROR", $"Error al llamar a la API meteorológica: {ex.Message}");
        }
        catch (Exception ex)
        {
            return SerializeError(idEspacio, "UNEXPECTED_ERROR", $"Error inesperado: {ex.Message}");
        }
    }
}
