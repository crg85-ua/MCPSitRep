using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using ModelContextProtocol.Server;

/// <summary>
/// Herramientas MCP para análisis de datos de sensores de aforo/ocupación con interpretación de IA.
/// Estas herramientas pueden llamar endpoints de APIs de sensores de aforo e interpretar los datos usando IA.
/// </summary>
internal class OccupancyAnalysisTools
{
    private readonly HttpClient _httpClient;
    private readonly IAIService _aiService;

    // URL base predefinida para la API
    private const string BaseApiUrl = "/GetRawData";

    public OccupancyAnalysisTools(HttpClient httpClient, IAIService aiService)
    {
        _httpClient = httpClient;
        _aiService = aiService;
    }

    /// <summary>
    /// Llama al endpoint predefinido de API de sensor de aforo usando el sensorId y usa IA (Llama) para interpretar los datos de ocupación,
    /// devolviendo análisis estructurado en formato JSON con porcentaje de ocupación y recomendaciones
    /// </summary>
    /// <param name="sensorId">ID del sensor de aforo a consultar</param>
    /// <param name="apiKey">Clave API opcional para autenticación</param>
    /// <param name="includeFlowAnalysis">Incluir análisis de flujo de personas (entradas/salidas)</param>
    /// <param name="includeAlerts">Incluir alertas y recomendaciones operativas en el análisis</param>
    /// <returns>Respuesta JSON estructurada con análisis completo de ocupación</returns>
    [McpServerTool]
    [Description("Llama al endpoint predefinido de API de sensor de aforo usando el sensorId y usa IA (Llama) para interpretar los datos de ocupación, devolviendo análisis estructurado con porcentaje de ocupación y recomendaciones operativas.")]
    public async Task<string> GetOccupancyDataWithAIAnalysis(
        [Description("ID del sensor de aforo a consultar")] string sensorId,
        [Description("Clave API opcional para autenticación")] string? apiKey = null,
        [Description("Incluir análisis de flujo de personas (entradas/salidas)")] bool includeFlowAnalysis = true,
        [Description("Incluir alertas y recomendaciones operativas en el análisis")] bool includeAlerts = true)
    {
        try
        {
            // Construir la URL predefinida con el sensorId
            var apiUrl = $"{BaseApiUrl}/GetBeachDataBySensor/{sensorId}";

            // Configurar headers de autenticación si se proporciona API key
            if (!string.IsNullOrEmpty(apiKey))
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            }

            // Llamar a la API del sensor de aforo
            var response = await _httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            var occupancyData = await response.Content.ReadAsStringAsync();

            // Intentar parsear los datos JSON del sensor
            OccupancySensorData? sensorData = null;
            try
            {
                sensorData = await response.Content.ReadFromJsonAsync<OccupancySensorData>();
            }
            catch
            {
                // Si falla el parsing, trabajamos con datos brutos
            }

            // Generar análisis de IA usando el modelo Llama real
            var analysis = await _aiService.AnalyzeOccupancyDataAsync(occupancyData, sensorData, includeFlowAnalysis, includeAlerts);

            // Retornar respuesta JSON estructurada
            var result = new
            {
                success = true,
                timestamp = DateTime.UtcNow,
                sensorId = sensorId,
                apiEndpoint = apiUrl,
                dataType = "OCCUPANCY_SENSOR_DATA",
                rawData = occupancyData,
                parsedData = sensorData,
                aiAnalysis = analysis
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
                message = $"Error al llamar a la API del sensor de aforo: {ex.Message}",
                details = new { endpoint = $"{BaseApiUrl}/GetBeachDataBySensor/{sensorId}", statusCode = ex.Data["StatusCode"]?.ToString() }
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

    /// <summary>
    /// Simula una llamada a una API de sensor de aforo con datos de prueba y usa IA real (Llama) para el análisis,
    /// devolviendo resultados estructurados en JSON con análisis de ocupación y recomendaciones
    /// </summary>
    /// <param name="currentOccupancy">Número actual de personas detectadas en el espacio</param>
    /// <param name="maxCapacity">Capacidad máxima del espacio</param>
    /// <param name="entrances">Número de entradas detectadas en el período</param>
    /// <param name="exits">Número de salidas detectadas en el período</param>
    /// <param name="zone">Zona o área específica del sensor</param>
    /// <param name="includeAlerts">Incluir alertas y recomendaciones operativas</param>
    /// <returns>Respuesta JSON con análisis estructurado de datos simulados</returns>
    [McpServerTool]
    [Description("Simula una llamada a una API de sensor de aforo con datos de prueba y usa IA real (Llama) para el análisis, devolviendo análisis estructurado de ocupación con porcentaje y recomendaciones.")]
    public async Task<string> GetMockOccupancyDataWithAIAnalysis(
        [Description("Número actual de personas detectadas en el espacio")] int currentOccupancy = 45,
        [Description("Capacidad máxima del espacio")] int maxCapacity = 60,
        [Description("Número de entradas detectadas en el período")] int entrances = 12,
        [Description("Número de salidas detectadas en el período")] int exits = 8,
        [Description("Zona o área específica del sensor")] string zone = "Zona Principal",
        [Description("Incluir alertas y recomendaciones operativas")] bool includeAlerts = true)
    {
        try
        {
            var mockData = new OccupancySensorData
            {
                Id = "urn:ngsi-ld:Beach:demo-001",
                Type = "Beach",
                Name = "Zona Principal de Aforo",
                Description = "Zona de prueba para análisis de ocupación",
                DateCreated = DateTime.UtcNow.AddDays(-30),
                DateModified = DateTime.UtcNow,
                DateObserved = DateTime.UtcNow,
                SensorId = "AFORO_SENSOR_001",
                PeopleOccupancy = currentOccupancy,
                MaximumCapacity = maxCapacity,
                Entrances = entrances,
                Exits = exits,
                Zone = zone,
                SensorStatus = "active",
                OccupancyStatus = currentOccupancy >= maxCapacity ? "full" : "available",
                Category = new[] { "public" },
                Facilities = new[] { "accessforDisabled", "toilets", "cleaningServices" },
                Location = new GeoJsonGeometry
                {
                    Type = "Point",
                    Coordinates = new double[] { -0.5, 38.5 } // Ejemplo: Valencia, España
                },
                Address = new Address
                {
                    StreetAddress = "Calle de Prueba 123",
                    AddressLocality = "Valencia",
                    AddressRegion = "Comunidad Valenciana",
                    AddressCountry = "España",
                    PostalCode = "46000"
                },
                AreaServed = "Valencia Centro",
                DataProvider = "MCPSitRep Occupancy Monitoring System"
            };

            var mockJson = JsonSerializer.Serialize(mockData, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });

            // Usar servicio de IA real para interpretación
            var analysis = await _aiService.AnalyzeOccupancyDataAsync(mockJson, mockData, true, includeAlerts);

            // Retornar respuesta JSON estructurada
            var result = new
            {
                success = true,
                timestamp = DateTime.UtcNow,
                dataType = "MOCK_OCCUPANCY_DATA",
                simulatedData = mockData,
                aiAnalysis = analysis,
                calculatedMetrics = new
                {
                    occupancyPercentage = mockData.OccupancyPercentage,
                    occupancyLevel = mockData.OccupancyLevel,
                    availableSpaces = maxCapacity - currentOccupancy,
                    netFlow = entrances - exits
                }
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
                message = $"Error procesando datos simulados de aforo: {ex.Message}",
                details = new { exception = ex.GetType().Name }
            };

            return JsonSerializer.Serialize(errorResult, new JsonSerializerOptions 
            { 
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }

    /// <summary>
    /// Calcula métricas básicas de ocupación para un espacio dado
    /// Útil para verificar cálculos o análisis rápidos sin IA
    /// </summary>
    /// <param name="currentOccupancy">Ocupación actual</param>
    /// <param name="maxCapacity">Capacidad máxima</param>
    /// <param name="entrances">Entradas en período</param>
    /// <param name="exits">Salidas en período</param>
    /// <returns>Métricas calculadas en formato JSON</returns>
    [McpServerTool]
    [Description("Calcula métricas básicas de ocupación (porcentaje, nivel, espacios disponibles) para análisis rápido sin usar IA.")]
    public string CalculateOccupancyMetrics(
        [Description("Ocupación actual del espacio")] int currentOccupancy,
        [Description("Capacidad máxima del espacio")] int maxCapacity,
        [Description("Entradas detectadas en el período")] int entrances = 0,
        [Description("Salidas detectadas en el período")] int exits = 0)
    {
        try
        {
            var occupancyPercentage = maxCapacity > 0 ? Math.Min(100, Math.Max(0, (double)currentOccupancy / maxCapacity * 100)) : 0;
            var availableSpaces = Math.Max(0, maxCapacity - currentOccupancy);
            var netFlow = entrances - exits;
            
            var occupancyLevel = occupancyPercentage switch
            {
                <= 25 => "Bajo",
                <= 50 => "Moderado", 
                <= 75 => "Alto",
                <= 90 => "Muy Alto",
                _ => "Completo"
            };

            var accessRecommendation = occupancyPercentage switch
            {
                < 80 => "Permitir acceso",
                >= 80 and < 95 => "Restringir acceso",
                _ => "Denegar acceso"
            };

            var result = new
            {
                success = true,
                timestamp = DateTime.UtcNow,
                metrics = new
                {
                    currentOccupancy,
                    maxCapacity,
                    occupancyPercentage = Math.Round(occupancyPercentage, 1),
                    occupancyLevel,
                    availableSpaces,
                    entrances,
                    exits,
                    netFlow,
                    accessRecommendation
                }
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
                error = "CALCULATION_ERROR",
                message = $"Error calculando métricas de ocupación: {ex.Message}"
            };

            return JsonSerializer.Serialize(errorResult, new JsonSerializerOptions 
            { 
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }
}