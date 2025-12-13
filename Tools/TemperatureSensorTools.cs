using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using ModelContextProtocol.Server;

/// <summary>
/// Herramientas MCP para recolección de datos de sensores de temperatura e interpretación con IA.
/// Estas herramientas pueden llamar endpoints de APIs de sensores de temperatura e interpretar los datos usando IA.
/// </summary>
internal class TemperatureSensorTools
{
    private readonly HttpClient _httpClient;
    private readonly IAIService _aiService;

    // URL base predefinida para la API
    private const string BaseApiUrl = "/GetRawData";

    public TemperatureSensorTools(HttpClient httpClient, IAIService aiService)
    {
        _httpClient = httpClient;
        _aiService = aiService;
    }

    [McpServerTool]
    [Description("Llama al endpoint predefinido de API de pronóstico meteorológico usando el sensorId y usa IA (Llama) para interpretar los datos completos del clima, devolviendo un análisis estructurado en formato JSON.")]
    public async Task<string> GetTemperatureDataWithAIInterpretation(
        [Description("ID del sensor de temperatura a consultar")] string sensorId,
        [Description("Clave API opcional para autenticación")] string? apiKey = null,
        [Description("Incluir recomendaciones de seguridad en la interpretación")] bool includeSafetyRecommendations = true,
        [Description("Incluir análisis de tendencias si está disponible")] bool includeTrendAnalysis = true)
    {
        try
        {
            // Construir la URL predefinida con el sensorId
            var apiUrl = $"{BaseApiUrl}/GetTemperatureDataBySensor/{sensorId}";

            // Configure HTTP client headers if API key is provided
            if (!string.IsNullOrEmpty(apiKey))
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            }

            // Call the temperature sensor API
            var response = await _httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            var temperatureData = await response.Content.ReadAsStringAsync();

            // Try to parse as JSON to extract temperature values
            TemperatureSensorData? sensorData = null;
            try
            {
                sensorData = await response.Content.ReadFromJsonAsync<TemperatureSensorData>();
            }
            catch
            {
                // If parsing fails, we'll work with raw data
            }

            // Generate AI interpretation using REAL Llama model
            var interpretation = await _aiService.AnalyzeTemperatureDataAsync(temperatureData, sensorData, includeSafetyRecommendations, includeTrendAnalysis);

            // Return structured JSON response
            var result = new
            {
                success = true,
                timestamp = DateTime.UtcNow,
                sensorId = sensorId,
                apiEndpoint = apiUrl,
                rawData = temperatureData,
                parsedData = sensorData,
                aiAnalysis = interpretation
            };

            return JsonSerializer.Serialize(result, new JsonSerializerOptions 
            { 
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        catch (HttpRequestException ex)
        {
            var errorResult = new
            {
                success = false,
                timestamp = DateTime.UtcNow,
                sensorId = sensorId,
                error = "API_CONNECTION_ERROR",
                message = $"Error al llamar a la API del sensor de temperatura: {ex.Message}",
                details = new { endpoint = $"{BaseApiUrl}/GetTemperatureDataBySensor/{sensorId}", statusCode = ex.Data["StatusCode"]?.ToString() }
            };

            return JsonSerializer.Serialize(errorResult, new JsonSerializerOptions 
            { 
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        catch (Exception ex)
        {
            var errorResult = new
            {
                success = false,
                timestamp = DateTime.UtcNow,
                sensorId = sensorId,
                error = "UNEXPECTED_ERROR",
                message = $"Error inesperado: {ex.Message}",
                details = new { exception = ex.GetType().Name }
            };

            return JsonSerializer.Serialize(errorResult, new JsonSerializerOptions 
            { 
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }

    [McpServerTool]
    [Description("Simula una llamada a una API de pronóstico meteorológico con datos de prueba completos y usa IA real (Llama) para el análisis, devolviendo resultados estructurados en JSON.")]
    public async Task<string> GetMockTemperatureDataWithAIInterpretation(
        [Description("Valor simulado de temperatura en Celsius")] double temperatureCelsius = 22.5,
        [Description("Humedad relativa simulada (0.0-1.0)")] double relativeHumidity = 0.45,
        [Description("Velocidad del viento simulada en m/s")] double windSpeed = 3.2,
        [Description("Tipo de clima simulado")] string weatherType = "soleado",
        [Description("Precipitación simulada en mm")] double precipitation = 0.0,
        [Description("Incluir recomendaciones de seguridad en la interpretación")] bool includeSafetyRecommendations = true)
    {
        try
        {
            var mockData = new TemperatureSensorData
            {
                Id = "urn:ngsi-ld:WeatherForecast:demo-001",
                Type = "WeatherForecast",
                DateIssued = DateTime.UtcNow,
                ValidFrom = DateTime.UtcNow,
                ValidTo = DateTime.UtcNow.AddHours(24),
                DateRetrieved = DateTime.UtcNow,
                Temperature = temperatureCelsius,
                RelativeHumidity = relativeHumidity,
                WindSpeed = windSpeed,
                WeatherType = weatherType,
                Precipitation = precipitation,
                Location = new GeoJsonGeometry
                {
                    Type = "Point",
                    Coordinates = new double[] { -0.5, 38.5 } // Ejemplo: Valencia, España
                },
                AreaServed = "Valencia, España",
                DataProvider = "MCPSitRep Mock Data Provider"
            };

            var mockJson = JsonSerializer.Serialize(mockData, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });

            // Use REAL AI service for interpretation
            var interpretation = await _aiService.AnalyzeTemperatureDataAsync(mockJson, mockData, includeSafetyRecommendations, true);

            // Return structured JSON response
            var result = new
            {
                success = true,
                timestamp = DateTime.UtcNow,
                dataType = "MOCK_DATA",
                simulatedData = mockData,
                aiAnalysis = interpretation
            };

            return JsonSerializer.Serialize(result, new JsonSerializerOptions 
            { 
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        catch (Exception ex)
        {
            var errorResult = new
            {
                success = false,
                timestamp = DateTime.UtcNow,
                error = "MOCK_DATA_ERROR",
                message = $"Error procesando datos simulados: {ex.Message}",
                details = new { exception = ex.GetType().Name }
            };

            return JsonSerializer.Serialize(errorResult, new JsonSerializerOptions 
            { 
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }
}