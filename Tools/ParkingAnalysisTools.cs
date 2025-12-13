using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using ModelContextProtocol.Server;

/// <summary>
/// Herramientas MCP para análisis de datos de sensores de aparcamiento con interpretación de IA.
/// Estas herramientas pueden llamar endpoints de APIs de sensores de aparcamiento e interpretar los datos usando IA.
/// </summary>
internal class ParkingAnalysisTools
{
    private readonly HttpClient _httpClient;
    private readonly IAIService _aiService;

    // URL base predefinida para la API
    private const string BaseApiUrl = "/GetRawData";

    public ParkingAnalysisTools(HttpClient httpClient, IAIService aiService)
    {
        _httpClient = httpClient;
        _aiService = aiService;
    }

    /// <summary>
    /// Llama al endpoint predefinido de API de sensores de aparcamiento usando el sensorId y usa IA (Llama) para interpretar los datos,
    /// indicando cuántas plazas están ocupadas y cuántas están disponibles con análisis detallado
    /// </summary>
    /// <param name="sensorId">ID del sensor de aparcamiento a consultar</param>
    /// <param name="apiKey">Clave API opcional para autenticación</param>
    /// <param name="includeViolationAnalysis">Incluir análisis de infracciones de tiempo de aparcamiento</param>
    /// <param name="includeSensorHealth">Incluir análisis del estado de salud de los sensores</param>
    /// <returns>Respuesta JSON estructurada con análisis completo de aparcamiento</returns>
    [McpServerTool]
    [Description("Llama al endpoint predefinido de API de sensores de aparcamiento usando el sensorId y usa IA (Llama) para interpretar los datos, indicando plazas ocupadas, disponibles y análisis de gestión.")]
    public async Task<string> GetParkingDataWithAIAnalysis(
        [Description("ID del sensor de aparcamiento a consultar")] string sensorId,
        [Description("Clave API opcional para autenticación")] string? apiKey = null,
        [Description("Incluir análisis de infracciones de tiempo de aparcamiento")] bool includeViolationAnalysis = true,
        [Description("Incluir análisis del estado de salud de los sensores")] bool includeSensorHealth = true)
    {
        try
        {
            // Construir la URL predefinida con el sensorId
            var apiUrl = $"{BaseApiUrl}/GetParkingSpotDataBySensor/{sensorId}";

            // Configurar headers de autenticación si se proporciona API key
            if (!string.IsNullOrEmpty(apiKey))
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            }

            // Llamar a la API de sensores de aparcamiento
            var response = await _httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            var parkingData = await response.Content.ReadAsStringAsync();

            // Intentar parsear los datos JSON de los sensores
            ParkingSensorCollection? sensorData = null;
            try
            {
                sensorData = await response.Content.ReadFromJsonAsync<ParkingSensorCollection>();
            }
            catch
            {
                // Si falla el parsing, intentar como array simple de sensores
                try
                {
                    var sensorArray = await response.Content.ReadFromJsonAsync<ParkingSensorData[]>();
                    if (sensorArray != null)
                    {
                        sensorData = new ParkingSensorCollection
                        {
                            ParkingSpots = new List<ParkingSensorData>(sensorArray),
                            LastUpdate = DateTime.UtcNow
                        };
                    }
                }
                catch
                {
                    // Si falla todo parsing, trabajamos con datos brutos
                }
            }

            // Generar análisis de IA usando el modelo Llama real
            var analysis = await _aiService.AnalyzeParkingDataAsync(parkingData, sensorData, includeViolationAnalysis, includeSensorHealth);

            // Calcular métricas básicas para incluir en la respuesta
            var basicMetrics = CalculateBasicParkingMetrics(sensorData);

            // Retornar respuesta JSON estructurada
            var result = new
            {
                success = true,
                timestamp = DateTime.UtcNow,
                sensorId = sensorId,
                apiEndpoint = apiUrl,
                dataType = "PARKING_SENSOR_DATA",
                rawData = parkingData,
                parsedData = sensorData,
                basicMetrics = basicMetrics,
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
                message = $"Error al llamar a la API de sensores de aparcamiento: {ex.Message}",
                details = new { endpoint = $"{BaseApiUrl}/GetParkingSpotDataBySensor/{sensorId}", statusCode = ex.Data["StatusCode"]?.ToString() }
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
    /// Simula una llamada a una API de sensores de aparcamiento con datos de prueba y usa IA real (Llama) para el análisis,
    /// devolviendo análisis detallado de disponibilidad y gestión de plazas
    /// </summary>
    /// <param name="totalSpots">Número total de plazas de aparcamiento a simular</param>
    /// <param name="occupiedSpots">Número de plazas ocupadas</param>
    /// <param name="outOfOrderSpots">Número de plazas fuera de servicio</param>
    /// <param name="areaName">Nombre del área de aparcamiento</param>
    /// <param name="zone">Zona o distrito del aparcamiento</param>
    /// <param name="includeViolations">Incluir simulación de infracciones de tiempo</param>
    /// <returns>Respuesta JSON con análisis estructurado de datos simulados</returns>
    [McpServerTool]
    [Description("Simula una llamada a una API de sensores de aparcamiento con datos de prueba y usa IA real (Llama) para análisis detallado de disponibilidad y gestión.")]
    public async Task<string> GetMockParkingDataWithAIAnalysis(
        [Description("Número total de plazas de aparcamiento a simular")] int totalSpots = 50,
        [Description("Número de plazas ocupadas")] int occupiedSpots = 35,
        [Description("Número de plazas fuera de servicio")] int outOfOrderSpots = 2,
        [Description("Nombre del área de aparcamiento")] string areaName = "Centro Comercial",
        [Description("Zona o distrito del aparcamiento")] string zone = "Zona Centro",
        [Description("Incluir simulación de infracciones de tiempo")] bool includeViolations = true)
    {
        try
        {
            var mockData = CreateMockParkingData(totalSpots, occupiedSpots, outOfOrderSpots, areaName, zone, includeViolations);

            var mockJson = JsonSerializer.Serialize(mockData, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });

            // Usar servicio de IA real para interpretación
            var analysis = await _aiService.AnalyzeParkingDataAsync(mockJson, mockData, includeViolations, true);

            // Calcular métricas básicas
            var basicMetrics = CalculateBasicParkingMetrics(mockData);

            // Retornar respuesta JSON estructurada
            var result = new
            {
                success = true,
                timestamp = DateTime.UtcNow,
                dataType = "MOCK_PARKING_DATA",
                simulatedData = mockData,
                basicMetrics = basicMetrics,
                aiAnalysis = analysis
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
                message = $"Error procesando datos simulados de aparcamiento: {ex.Message}",
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
    /// Calcula métricas básicas de aparcamiento para análisis rápido sin IA
    /// Útil para verificaciones rápidas de disponibilidad y ocupación
    /// </summary>
    /// <param name="totalSpots">Total de plazas monitorizadas</param>
    /// <param name="occupiedSpots">Plazas ocupadas</param>
    /// <param name="outOfOrderSpots">Plazas fuera de servicio</param>
    /// <returns>Métricas calculadas en formato JSON</returns>
    [McpServerTool]
    [Description("Calcula métricas básicas de aparcamiento (ocupación, disponibilidad, porcentajes) para análisis rápido sin usar IA.")]
    public string CalculateParkingMetrics(
        [Description("Número total de plazas monitorizadas")] int totalSpots,
        [Description("Número de plazas ocupadas")] int occupiedSpots,
        [Description("Número de plazas fuera de servicio")] int outOfOrderSpots = 0)
    {
        try
        {
            var availableSpots = Math.Max(0, totalSpots - occupiedSpots - outOfOrderSpots);
            var usableSpots = totalSpots - outOfOrderSpots;
            var occupancyPercentage = usableSpots > 0 ? Math.Round((double)occupiedSpots / usableSpots * 100, 1) : 0;
            var availabilityPercentage = usableSpots > 0 ? Math.Round((double)availableSpots / usableSpots * 100, 1) : 0;

            var availabilityLevel = occupancyPercentage switch
            {
                < 50 => "Alta",
                >= 50 and < 75 => "Media",
                >= 75 and < 90 => "Baja",
                _ => "Crítica"
            };

            var systemStatus = outOfOrderSpots > (totalSpots * 0.1) ? "Mantenimiento requerido" :
                               occupancyPercentage > 95 ? "Sobrecarga" :
                               occupancyPercentage < 20 ? "Infrautilizado" : "Normal";

            var result = new
            {
                success = true,
                timestamp = DateTime.UtcNow,
                metrics = new
                {
                    totalSpots,
                    occupiedSpots,
                    availableSpots,
                    outOfOrderSpots,
                    usableSpots,
                    occupancyPercentage,
                    availabilityPercentage,
                    availabilityLevel,
                    systemStatus
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
                message = $"Error calculando métricas de aparcamiento: {ex.Message}"
            };

            return JsonSerializer.Serialize(errorResult, new JsonSerializerOptions 
            { 
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }

    /// <summary>
    /// Crea datos simulados de sensores de aparcamiento para pruebas
    /// </summary>
    private ParkingSensorCollection CreateMockParkingData(int totalSpots, int occupiedSpots, int outOfOrderSpots, string areaName, string zone, bool includeViolations)
    {
        var random = new Random();
        var spots = new List<ParkingSensorData>();
        var spotTypes = new[] { "regular", "disabled", "electric", "motorcycle" };
        var availableSpots = totalSpots - occupiedSpots - outOfOrderSpots;

        for (int i = 1; i <= totalSpots; i++)
        {
            var status = "free";
            var timeSinceChange = random.Next(10, 300);
            
            if (i <= occupiedSpots)
            {
                status = "occupied";
                if (includeViolations && random.NextDouble() < 0.15) // 15% chance de infracción
                {
                    timeSinceChange = random.Next(120, 480); // 2-8 horas
                }
            }
            else if (i <= occupiedSpots + outOfOrderSpots)
            {
                status = "closed"; // FIWARE uses 'closed' instead of 'outOfOrder'
            }

            var spot = new ParkingSensorData
            {
                Id = $"urn:ngsi-ld:ParkingSpot:spot-{i:D3}",
                Type = "ParkingSpot",
                Name = $"Plaza {i:D3}",
                Description = $"Plaza de aparcamiento número {i}",
                DateCreated = DateTime.UtcNow.AddDays(-30),
                DateModified = DateTime.UtcNow,
                TimeInstant = DateTime.UtcNow,
                SensorId = $"PARKING_SENSOR_{i:D3}",
                Number = i.ToString("D3"),
                Status = status,
                TimeSinceLastChange = timeSinceChange,
                Zone = zone,
                SpotType = spotTypes[random.Next(spotTypes.Length)],
                Category = new[] { status == "occupied" ? "offstreet" : "onstreet" },
                HourlyRate = random.Next(1, 5) + random.NextDouble(),
                MaxParkingTime = random.Next(60, 240),
                SensorStatus = random.NextDouble() < 0.95 ? "active" : "error",
                BatteryLevel = random.Next(20, 100),
                Location = new GeoJsonGeometry
                {
                    Type = "Point",
                    Coordinates = new double[] { -0.5 + random.NextDouble() * 0.01, 38.5 + random.NextDouble() * 0.01 }
                },
                Address = new Address
                {
                    StreetAddress = $"Calle del Aparcamiento {i}",
                    AddressLocality = zone,
                    AddressRegion = "Comunidad Valenciana",
                    AddressCountry = "España",
                    PostalCode = "46000"
                },
                AreaServed = zone,
                DataProvider = "MCPSitRep Parking Management System"
            };

            spots.Add(spot);
        }

        return new ParkingSensorCollection
        {
            ParkingSpots = spots,
            AreaInfo = new ParkingAreaInfo
            {
                AreaName = areaName,
                TotalSpots = totalSpots,
                Zone = zone,
                ParkingType = "lot"
            },
            LastUpdate = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Calcula métricas básicas de una colección de sensores de aparcamiento
    /// </summary>
    private object? CalculateBasicParkingMetrics(ParkingSensorCollection? sensorData)
    {
        if (sensorData?.ParkingSpots == null || !sensorData.ParkingSpots.Any())
        {
            return null;
        }

        var totalSpots = sensorData.ParkingSpots.Count;
        var occupiedSpots = sensorData.ParkingSpots.Count(s => s.IsOccupied);
        var availableSpots = sensorData.ParkingSpots.Count(s => s.IsAvailable);
        var outOfOrderSpots = sensorData.ParkingSpots.Count(s => s.IsOutOfOrder);
        var violationSpots = sensorData.ParkingSpots.Count(s => s.OccupancyAlert == "Infracción");
        var warningSpots = sensorData.ParkingSpots.Count(s => s.OccupancyAlert == "Advertencia" || s.OccupancyAlert == "Crítico");

        var occupancyPercentage = totalSpots > 0 ? Math.Round((double)occupiedSpots / totalSpots * 100, 1) : 0;

        return new
        {
            totalSpots,
            occupiedSpots,
            availableSpots,
            outOfOrderSpots,
            occupancyPercentage,
            violationSpots,
            warningSpots,
            lastUpdate = sensorData.LastUpdate,
            areaName = sensorData.AreaInfo?.AreaName
        };
    }
}