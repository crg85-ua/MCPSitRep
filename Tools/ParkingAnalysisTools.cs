using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using Parking;

internal class ParkingAnalysisTools : AnalysisToolBase
{
    public ParkingAnalysisTools(HttpClient httpClient, IAIService aiService)
        : base(httpClient, aiService) { }

    [McpServerTool]
    [Description("Llama al endpoint de API de sensores de aparcamiento usando el IdEspacio y una fecha concreta, " +
        "y usa IA (Gemini) para interpretar los datos del espacio, " +
        "indicando plazas ocupadas, disponibles y análisis de gestión.")]
    public async Task<string> GetParkingAnalysisWithAI(
        [Description("ID del espacio de aparcamiento a consultar")] string idEspacio,
        [Description("Fecha de consulta en formato (dd-MM-yyyy). Si no se indica, se usa la fecha actual.")] string? fecha = null)
    {
        var fechaConsulta = DateTime.TryParse(fecha, out var parsedDate) ? parsedDate.Date : DateTime.UtcNow.Date;

        try
        {
            var apiUrl = $"{BaseApiUrl}/GetParkingDataBySpace/{idEspacio}?date={fechaConsulta:yyyy-MM-dd}";
            var response = await _httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            var rawData = await response.Content.ReadAsStringAsync();
            var sensorData = JsonSerializer.Deserialize<ParkingSensorCollection>(rawData, JsonOptions);
            var analysis = await _aiService.AnalyzeParkingDataAsync(rawData, sensorData);


            return SerializeSuccess(new
            {
                success = true,
                timestamp = DateTime.UtcNow,
                idEspacio,
                fecha = fechaConsulta.ToString("yyyy-MM-dd"),
                dataType = "PARKING_SPACE_DATA",
                aiAnalysis = analysis
            });
        }
        catch (HttpRequestException ex)
        {
            return SerializeError(idEspacio, "API_CONNECTION_ERROR", $"Error al llamar a la API de aparcamiento: {ex.Message}");
        }
        catch (Exception ex)
        {
            return SerializeError(idEspacio, "UNEXPECTED_ERROR", $"Error inesperado: {ex.Message}");
        }
    }
}
