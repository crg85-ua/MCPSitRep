using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Google.GenAI.Types;
using Google.GenAI;
using Occupancy;
using Parking;
using Weather;


/// <summary>
/// Servicio para realizar llamadas a modelos de IA como Llama
/// </summary>
public interface IAIService
{
    /// <summary>
    /// Analiza datos meteorol�gicos utilizando el modelo de IA Llama y devuelve un an�lisis estructurado
    /// </summary>
    /// <param name="temperatureData">Datos brutos del sensor en formato JSON string</param>
    /// <param name="parsedData">Datos del sensor parseados a objeto TemperatureSensorData (puede ser null)</param>
    /// <param name="includeSafetyRecommendations">Indica si incluir recomendaciones de seguridad en el an�lisis</param>
    /// <param name="includeTrendAnalysis">Indica si incluir an�lisis de tendencias en el resultado</param>
    /// <returns>Objeto WeatherAnalysisResult con el an�lisis completo o null si hay error</returns>
    Task<WeatherAnalysisResult?> AnalyzeTemperatureDataAsync(string temperatureData, TemperatureSensorData? parsedData, bool includeSafetyRecommendations, bool includeTrendAnalysis);

    /// <summary>
    /// Analiza datos de aforo/ocupaci�n utilizando el modelo de IA Llama y devuelve un an�lisis estructurado
    /// </summary>
    /// <param name="occupancyData">Datos brutos del sensor de aforo en formato JSON string</param>
    /// <param name="parsedData">Datos del sensor parseados a objeto OccupancySensorData (puede ser null)</param>
    /// <param name="includeAlerts">Indica si incluir alertas y recomendaciones operativas</param>
    /// <returns>Objeto OccupancyAnalysisResult con el an�lisis completo o null si hay error</returns>
    Task<OccupancyAnalysisResult?> AnalyzeOccupancyDataAsync(string occupancyData, OccupancySensorData? parsedData, bool includeAlerts);

    /// <summary>
    /// Analiza datos de sensores de aparcamiento utilizando el modelo de IA Llama y devuelve un an�lisis estructurado
    /// </summary>
    /// <param name="parkingData">Datos brutos de sensores de aparcamiento en formato JSON string</param>
    /// <param name="parsedData">Datos parseados de m�ltiples sensores (puede ser null)</param>
    /// <returns>Objeto ParkingAnalysisResult con el an�lisis completo o null si hay error</returns>
    Task<ParkingAnalysisResult?> AnalyzeParkingDataAsync(string parkingData, ParkingSensorCollection? parsedData);
}

/// <summary>
/// Implementaci�n del servicio de IA que se conecta a Gemini
/// </summary>
public class GeminiAIService : IAIService
{
    private readonly Client _client;
    private readonly string _modelName;
    private readonly ILogger<GeminiAIService> _logger;


    /// <summary>
    /// Constructor que inicializa el cliente de Gemini con la configuraci�n proporcionada
    /// </summary>
    /// <param name="configuration">Configuraci�n de la aplicaci�n que contiene endpoint y modelo de IA</param>
    /// <param name="logger">Logger para registrar errores y eventos del servicio</param>
    public GeminiAIService(IConfiguration configuration, ILogger<GeminiAIService> logger)
    {
        _logger = logger;

        // 1. Obtener la API Key desde la configuración
        var apiKey = configuration.GetValue<string>("GeminiAI:ApiKey")
                     ?? throw new ArgumentNullException("GeminiAI:ApiKey no encontrada.");

        _modelName = configuration.GetValue<string>("GeminiAI:ModelName") ?? "models/gemini-2.5-flash";

        // 2. Inicializar el cliente unificado de Google GenAI
        _client = new Client(apiKey: apiKey);
    }

    /// <summary>
    /// M�todo principal que analiza datos meteorol�gicos usando el modelo Llama
    /// Maneja la comunicaci�n con Gemini, procesa la respuesta JSON y proporciona fallbacks en caso de error
    /// </summary>
    /// <param name="temperatureData">Datos brutos del sensor meteorol�gico en formato JSON</param>
    /// <param name="parsedData">Objeto TemperatureSensorData parseado (opcional)</param>
    /// <param name="includeSafetyRecommendations">Flag para incluir recomendaciones de seguridad</param>
    /// <param name="includeTrendAnalysis">Flag para incluir an�lisis de tendencias</param>
    /// <returns>WeatherAnalysisResult con an�lisis completo o an�lisis b�sico en caso de error</returns>
    public async Task<WeatherAnalysisResult?> AnalyzeTemperatureDataAsync(string temperatureData, TemperatureSensorData? parsedData, bool includeSafetyRecommendations, bool includeTrendAnalysis)
    {
        try
        {
            var prompt = CreateWeatherAnalysisPrompt(temperatureData, parsedData, includeSafetyRecommendations, includeTrendAnalysis);

            // 3. La llamada se hace vía client.Models.GenerateContentAsync
            // Usamos GenerationConfig para forzar JSON
            var response = await _client.Models.GenerateContentAsync(_modelName, prompt, new()
            {
                ResponseMimeType = "application/json"
            });

            // 4. Se accede al texto mediante la propiedad .Text del objeto de respuesta
            var finalResponse = response?.Candidates?[0]?.Content?.Parts?[0]?.Text;

            if (!string.IsNullOrEmpty(finalResponse))
            {
                return ParseWeatherAnalysisResponse(finalResponse);
            }
            return null;
        }
        catch (HttpRequestException ex)
        {
            // Error de conexi�n con Gemini - devolver an�lisis b�sico como fallback
            return CreateWeatherFallbackAnalysis(parsedData, $"Error de conexion con Gemini: {ex.Message}");
        }
        catch (Exception ex)
        {
            // Error general - devolver an�lisis b�sico como fallback
            return CreateWeatherFallbackAnalysis(parsedData, $"Error al conectar con el modelo de Gemini: {ex.Message} {ex.StackTrace}");
        }
    }

    /// <summary>
    /// Analiza datos de aforo/ocupaci�n usando el modelo Llama
    /// Maneja la comunicaci�n con Ollama, procesa la respuesta JSON y proporciona fallbacks en caso de error
    /// </summary>
    /// <param name="occupancyData">Datos brutos del sensor de aforo en formato JSON</param>
    /// <param name="parsedData">Objeto OccupancySensorData parseado (opcional)</param>
    /// <param name="includeFlowAnalysis">Flag para incluir an�lisis de flujo de personas</param>
    /// <param name="includeAlerts">Flag para incluir alertas y recomendaciones operativas</param>
    /// <returns>OccupancyAnalysisResult con an�lisis completo o an�lisis b�sico en caso de error</returns>
    public async Task<OccupancyAnalysisResult?> AnalyzeOccupancyDataAsync(string occupancyData, OccupancySensorData? parsedData, bool includeAlerts)
    {
        try
        {
            var prompt = CreateOccupancyAnalysisPrompt(occupancyData, parsedData, includeAlerts);

            var response = await _client.Models.GenerateContentAsync(_modelName, prompt, new()
            {
                ResponseMimeType = "application/json"
            });

            // Se accede al texto mediante la propiedad .Text del objeto de respuesta
            var finalResponse = response?.Candidates?[0]?.Content?.Parts?[0]?.Text;

            if (!string.IsNullOrEmpty(finalResponse))
            {
                return ParseOccupancyAnalysisResponse(finalResponse);
            }
            return null;
        }
        catch (HttpRequestException ex)
        {
            // Error de conexi�n con Gemini - devolver an�lisis b�sico como fallback
            return CreateFallbackOccupancyAnalysis(parsedData, $"Error de conexion con Gemini: {ex.Message}");
        }
        catch (Exception ex)
        {
            // Error general - devolver an�lisis b�sico como fallback
            return CreateFallbackOccupancyAnalysis(parsedData, $"Error al conectar con el modelo de Gemini: {ex.Message} {ex.StackTrace}");
        }
    }

    /// <summary>
    /// Analiza datos de sensores de aparcamiento usando el modelo Llama
    /// Procesa m�ltiples sensores y proporciona an�lisis integral de disponibilidad y gesti�n
    /// </summary>
    /// <param name="parkingData">Datos brutos de sensores de aparcamiento en formato JSON</param>
    /// <param name="parsedData">Colecci�n de datos de sensores parseados (opcional)</param>
    /// <returns>ParkingAnalysisResult con an�lisis completo o an�lisis b�sico en caso de error</returns>
    public async Task<ParkingAnalysisResult?> AnalyzeParkingDataAsync(string parkingData, ParkingSensorCollection? parsedData)
    {
        try
        {
            var prompt = CreateParkingAnalysisPrompt(parkingData, parsedData);

            var response = await _client.Models.GenerateContentAsync(_modelName, prompt, new()
            {
                ResponseMimeType = "application/json"
            });

            // 4. Se accede al texto mediante la propiedad .Text del objeto de respuesta
            var finalResponse = response?.Candidates?[0]?.Content?.Parts?[0]?.Text;

            if (!string.IsNullOrEmpty(finalResponse))
            {
                return ParseParkingAnalysisResponse(finalResponse);
            }
            return null;
        }
        catch (HttpRequestException ex)
        {
            var errorMsg = $"Error de conexion con Gemini: {ex.Message}";
            _logger.LogError(ex, "AnalyzeParkingDataAsync - HttpRequestException: {Message}", ex.Message);
            return CreateFallbackParkingAnalysis(parsedData, errorMsg);
        }
        catch (Exception ex)
        {
            var errorMsg = $"Error al conectar con el modelo de Gemini: {ex.GetType().Name} - {ex.Message}";
            _logger.LogError(ex, "AnalyzeParkingDataAsync - Exception: {Type} {Message}", ex.GetType().Name, ex.Message);
            return CreateFallbackParkingAnalysis(parsedData, errorMsg);
        }
    }

    // Response parsing methods
    private WeatherAnalysisResult? ParseWeatherAnalysisResponse(string jsonResponse)
    {
        try
        {
            var cleanedResponse = ExtractJsonFromResponse(jsonResponse);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true
            };
            return JsonSerializer.Deserialize<WeatherAnalysisResult>(cleanedResponse, options);
        }
        catch (JsonException ex)
        {
            Console.Error.WriteLine($"Error parsing weather JSON response: {ex.Message}");
            return null;
        }
    }

    private OccupancyAnalysisResult? ParseOccupancyAnalysisResponse(string jsonResponse)
    {
        try
        {
            var cleanedResponse = ExtractJsonFromResponse(jsonResponse);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true
            };
            return JsonSerializer.Deserialize<OccupancyAnalysisResult>(cleanedResponse, options);
        }
        catch (JsonException ex)
        {
            Console.Error.WriteLine($"Error parsing occupancy JSON response: {ex.Message}");
            return null;
        }
    }

    private ParkingAnalysisResult? ParseParkingAnalysisResponse(string jsonResponse)
    {
        try
        {
            var cleanedResponse = ExtractJsonFromResponse(jsonResponse);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true
            };
            return JsonSerializer.Deserialize<ParkingAnalysisResult>(cleanedResponse, options);
        }
        catch (JsonException ex)
        {
            Console.Error.WriteLine($"Error parsing parking JSON response: {ex.Message}");
            return null;
        }
    }

    private string ExtractJsonFromResponse(string response)
    {
        var startIndex = response.IndexOf('{');
        var lastIndex = response.LastIndexOf('}');

        if (startIndex >= 0 && lastIndex > startIndex)
        {
            return response.Substring(startIndex, lastIndex - startIndex + 1);
        }

        return response;
    }

    // Fallback analysis methods
    private WeatherAnalysisResult CreateWeatherFallbackAnalysis(TemperatureSensorData? parsedData, string errorMessage)
    {
        var fallback = new WeatherAnalysisResult();

        if (parsedData != null)
        {
            fallback.TemperatureAnalysis = new TemperatureAnalysis
            {
                CurrentTemperature = parsedData.Temperature,
                TemperatureFahrenheit = parsedData.Temperature.HasValue ? (parsedData.Temperature * 9 / 5 + 32) : null,
                ComfortLevel = GetBasicComfortLevel(parsedData.Temperature),
                ComfortDescription = "Analysis result - IA no disponible",
                ComfortScore = GetBasicComfortScore(parsedData.Temperature)
            };

            fallback.HumidityAnalysis = new HumidityAnalysis
            {
                RelativeHumidity = parsedData.RelativeHumidity,
                HumidityPercentage = parsedData.Humidity,
                HumidityStatus = GetBasicHumidityStatus(parsedData.Humidity),
                HumidityDescription = "Analysis result - IA no disponible",
                HumidityScore = GetBasicHumidityScore(parsedData.Humidity)
            };

            fallback.WeatherConditions = new WeatherConditionsAnalysis
            {
                WindSpeed = parsedData.WindSpeed,
                WeatherType = parsedData.WeatherType,
                Precipitation = parsedData.Precipitation,
                OverallWeatherStatus = "fair",
                WeatherDescription = $"Analysis result - {errorMessage}",
                WeatherScore = 5
            };
        }

        return fallback;
    }

    private OccupancyAnalysisResult CreateFallbackOccupancyAnalysis(OccupancySensorData? parsedData, string errorMessage)
    {
        var fallback = new OccupancyAnalysisResult();

        if (parsedData != null)
        {
            fallback.OccupancyAnalysis = new OccupancyAnalysis
            {
                CurrentOccupancy = parsedData.CurrentOccupancy,
                MaxCapacity = parsedData.MaxCapacity,
                OccupancyPercentage = parsedData.OccupancyPercentage,
                OccupancyLevel = parsedData.OccupancyLevel.ToLowerInvariant(),
                StatusDescription = "An�lisis b�sico - IA no disponible",
                EfficiencyScore = GetBasicOccupancyScore(parsedData.OccupancyPercentage)
            };

            if (parsedData.MaxCapacity.HasValue && parsedData.CurrentOccupancy.HasValue)
            {
                fallback.CapacityAnalysis = new CapacityAnalysis
                {
                    AvailableSpaces = Math.Max(0, parsedData.MaxCapacity.Value - parsedData.CurrentOccupancy.Value),
                    AccessRecommendation = GetBasicAccessRecommendation(parsedData.OccupancyPercentage),
                    RecommendationDescription = $"An�lisis b�sico - {errorMessage}"
                };
            }
        }

        return fallback;
    }

    private ParkingAnalysisResult CreateFallbackParkingAnalysis(ParkingSensorCollection? parsedData, string errorMessage)
    {
        var fallback = new ParkingAnalysisResult
        {
            OccupancyAnalysis = new ParkingOccupancyAnalysis
            {
                StatusDescription = $"An�lisis b�sico - IA no disponible: {errorMessage}"
            }
        };

        if (parsedData?.ParkingSpots != null)
        {
            var totalSpots = parsedData.ParkingSpots.Count;
            var occupiedSpots = parsedData.ParkingSpots.Count(s => s.Status == "occupied");
            var availableSpots = parsedData.ParkingSpots.Count(s => s.Status == "free");
            var occupancyPercentage = totalSpots > 0 ? (double)occupiedSpots / totalSpots * 100 : 0;

            fallback.OccupancyAnalysis = new ParkingOccupancyAnalysis
            {
                TotalSpots = totalSpots,
                OccupiedSpots = occupiedSpots,
                AvailableSpots = availableSpots,
                OccupancyPercentage = occupancyPercentage,
                AvailabilityLevel = GetBasicAvailabilityLevel(occupancyPercentage),
                StatusDescription = $"An�lisis b�sico - IA no disponible: {errorMessage}"
            };
        }

        return fallback;
    }

    // Helper methods
    private string GetBasicComfortLevel(double? temperature)
    {
        if (!temperature.HasValue) return "unknown";
        if (temperature >= 20 && temperature <= 25) return "optimal";
        if (temperature >= 18 && temperature <= 27) return "acceptable";
        if (temperature < 18) return "cold";
        return "hot";
    }

    private int GetBasicComfortScore(double? temperature)
    {
        if (!temperature.HasValue) return 5;
        if (temperature >= 20 && temperature <= 25) return 10;
        if (temperature >= 18 && temperature <= 27) return 7;
        if (temperature < 10 || temperature > 35) return 1;
        return 4;
    }

    private string GetBasicHumidityStatus(double? humidity)
    {
        if (!humidity.HasValue) return "unknown";
        if (humidity >= 40 && humidity <= 60) return "ideal";
        if (humidity < 40) return "dry";
        return "humid";
    }

    private int GetBasicHumidityScore(double? humidity)
    {
        if (!humidity.HasValue) return 5;
        if (humidity >= 40 && humidity <= 60) return 10;
        if (humidity < 20 || humidity > 80) return 2;
        return 6;
    }

    private int GetBasicOccupancyScore(double? occupancyPercentage)
    {
        if (!occupancyPercentage.HasValue) return 5;

        return occupancyPercentage.Value switch
        {
            >= 70 and <= 85 => 10,
            >= 50 and < 70 => 8,
            >= 85 and <= 95 => 7,
            >= 25 and < 50 => 6,
            < 25 => 4,
            _ => 2
        };
    }

    private string GetBasicAccessRecommendation(double? occupancyPercentage)
    {
        if (!occupancyPercentage.HasValue) return "allow";

        return occupancyPercentage.Value switch
        {
            < 80 => "allow",
            >= 80 and < 95 => "restrict",
            _ => "deny"
        };
    }

    private string GetBasicAvailabilityLevel(double occupancyPercentage)
    {
        return occupancyPercentage switch
        {
            < 50 => "high",
            >= 50 and < 75 => "medium",
            >= 75 and < 90 => "low",
            _ => "critical"
        };
    }

    // Prompt creation methods
    private string CreateWeatherAnalysisPrompt(string rawData, TemperatureSensorData? parsedData, bool includeSafetyRecommendations, bool includeTrendAnalysis)
    {
        var safetySection = includeSafetyRecommendations ? @"""safetyRecommendations"": {
        ""riskLevel"": ""low|moderate|high|extreme"",
        ""alerts"": [""string1"", ""string2""],
        ""recommendedActions"": [""string1"", ""string2""],
        ""healthConsiderations"": [""string1"", ""string2""]
      }," : "";

        var trendSection = includeTrendAnalysis ? @"""trendAnalysis"": {
        ""temperatureTrend"": ""rising|falling|stable"",
        ""shortTermForecast"": ""string"",
        ""identifiedPatterns"": [""string1"", ""string2""],
        ""monitoringRecommendations"": [""string1"", ""string2""]
      }," : "";

        var temperatureInfo = parsedData?.Temperature.HasValue == true ? $"- Temperatura: {parsedData.Temperature:F1}�C" : "";
        var humidityInfo = parsedData?.Humidity.HasValue == true ? $"- Humedad: {parsedData.Humidity:F1}%" : "";
        var windInfo = parsedData?.WindSpeed.HasValue == true ? $"- Velocidad del viento: {parsedData.WindSpeed:F1} m/s" : "";
        var weatherInfo = parsedData?.WeatherType != null ? $"- Tipo de clima: {parsedData.WeatherType}" : "";
        var precipitationInfo = parsedData?.Precipitation.HasValue == true ? $"- Precipitaci�n: {parsedData.Precipitation:F1} mm" : "";
        var locationInfo = parsedData?.Location?.Coordinates != null && parsedData.Location.Coordinates.Length >= 2
            ? $"- Coordenadas: {parsedData.Location.Coordinates[1]:F4}, {parsedData.Location.Coordinates[0]:F4}" : "";

        var prompt = $@"Analiza los siguientes datos de pronóstico meteorológico y responde ÚNICAMENTE con un objeto JSON estructurado.

            DATOS DEL SENSOR: {rawData}

            ESQUEMA JSON REQUERIDO:
            {{""temperatureAnalysis"":
                    {{""currentTemperature"":number,
                    ""temperatureFahrenheit"":number,
                    ""comfortLevel"":""optimal|acceptable|cold|hot"",
                    ""comfortDescription"":""string"",
                    ""comfortScore"":number}},
             ""humidityAnalysis"":{{
                    ""relativeHumidity"":number,
                    ""humidityPercentage"":number,
                    ""humidityStatus"":""ideal|dry|humid"",
                    ""humidityDescription"":""string"",
                    ""humidityScore"":number}},
             ""weatherConditions"":{{""windSpeed"":number,
                    ""windCategory"":""calm|light|moderate|strong"",
                    ""weatherType"":""string"",""precipitation"":number,
                    ""overallWeatherStatus"":""excellent|good|fair|poor"",
                    ""weatherDescription"":""string"",""weatherScore"":number}},
                {safetySection}{trendSection}
                ""activityRecommendations"": {{
                    ""outdoorActivities"":[""string1""],
                    ""indoorActivities"":[""string1""],
                    ""activitiesToAvoid"":[""string1""],
                    ""recommendedClothing"":[""string1""],
                    ""outdoorSuitabilityScore"":number}}}}

            DATOS: {temperatureInfo} {humidityInfo} {windInfo} {weatherInfo} {precipitationInfo} {locationInfo}";

        return prompt;
    }

    private string CreateOccupancyAnalysisPrompt(string rawData, OccupancySensorData? parsedData, bool includeAlerts)
    {
        var alertsSection = includeAlerts ? @"""alerts"":{""alertLevel"":""none|low|medium|high|critical"",""activeAlerts"":[""string1""],""warnings"":[""string1""],""timeToNextAlert"":""string""}," : "";

        var occupancyInfo = parsedData?.CurrentOccupancy.HasValue == true ? $"Ocupaci�n:{parsedData.CurrentOccupancy}" : "";
        var capacityInfo = parsedData?.MaxCapacity.HasValue == true ? $"Capacidad:{parsedData.MaxCapacity}" : "";

        var prompt = $@"Analiza datos de aforo y responde SOLO JSON:

        DATOS: {rawData}

        ESQUEMA:{{""occupancyAnalysis"":{{""currentOccupancy"":number,""maxCapacity"":number,""occupancyPercentage"":number,""occupancyLevel"":""low|moderate|high|critical|full"",""statusDescription"":""string"",""efficiencyScore"":number}},""capacityAnalysis"":{{""availableSpaces"":number,""accessRecommendation"":""allow|restrict|deny"",""recommendationDescription"":""string""}},{alertsSection}""operationalRecommendations"":{{""immediateActions"":[""string1""],""queueManagement"":[""string1""],""userCommunications"":[""string1""]}}}}

        INFO: {occupancyInfo} {capacityInfo}";

        return prompt;
    }

    private string CreateParkingAnalysisPrompt(string rawData, ParkingSensorCollection? parsedData)
    {
        var totalSpots = parsedData?.ParkingSpots?.Count ?? 0;
        var parkingSpots = parsedData?.ParkingSpots?
                .Select(s => new { id = s.Id, status = s.Status, is_pmr = s.Is_pmr == true ? "PMR" : "Standard" })
                .ToList();
        var occupiedSpots = parsedData?.ParkingSpots?.Count(s => s.Status == "occupied") ?? 0;
        var summaryInfo = totalSpots > 0 ? $"Total:{totalSpots} Ocupadas:{occupiedSpots}" : "";

        var prompt = $@"Analiza datos de aparcamiento y responde SOLO CON JSON:

DATOS: {rawData}

ESQUEMA:{{""occupancyAnalysis"":{{""totalSpots"":number,""occupiedSpots"":number,""availableSpots"":number,""occupancyPercentage"":number,""availabilityLevel"":""high|medium|low|critical"",""statusDescription"":""string"",""parkingSpots"": [{{""id"":""string"",""status"":""occupied|free"",""type"":""PMR|Standard""}}]}}}}

INFO: {summaryInfo}";

        return prompt;
    }
}