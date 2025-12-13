using Microsoft.Extensions.Configuration;
using OllamaSharp;
using OllamaSharp.Models;
using System.Text;
using System.Text.Json;

/// <summary>
/// Servicio para realizar llamadas a modelos de IA como Llama
/// </summary>
public interface IAIService
{
    /// <summary>
    /// Analiza datos meteorológicos utilizando el modelo de IA Llama y devuelve un análisis estructurado
    /// </summary>
    /// <param name="temperatureData">Datos brutos del sensor en formato JSON string</param>
    /// <param name="parsedData">Datos del sensor parseados a objeto TemperatureSensorData (puede ser null)</param>
    /// <param name="includeSafetyRecommendations">Indica si incluir recomendaciones de seguridad en el análisis</param>
    /// <param name="includeTrendAnalysis">Indica si incluir análisis de tendencias en el resultado</param>
    /// <returns>Objeto WeatherAnalysisResult con el análisis completo o null si hay error</returns>
    Task<WeatherAnalysisResult?> AnalyzeTemperatureDataAsync(string temperatureData, TemperatureSensorData? parsedData, bool includeSafetyRecommendations, bool includeTrendAnalysis);

    /// <summary>
    /// Analiza datos de aforo/ocupación utilizando el modelo de IA Llama y devuelve un análisis estructurado
    /// </summary>
    /// <param name="occupancyData">Datos brutos del sensor de aforo en formato JSON string</param>
    /// <param name="parsedData">Datos del sensor parseados a objeto OccupancySensorData (puede ser null)</param>
    /// <param name="includeFlowAnalysis">Indica si incluir análisis de flujo de personas en el resultado</param>
    /// <param name="includeAlerts">Indica si incluir alertas y recomendaciones operativas</param>
    /// <returns>Objeto OccupancyAnalysisResult con el análisis completo o null si hay error</returns>
    Task<OccupancyAnalysisResult?> AnalyzeOccupancyDataAsync(string occupancyData, OccupancySensorData? parsedData, bool includeFlowAnalysis, bool includeAlerts);

    /// <summary>
    /// Analiza datos de sensores de aparcamiento utilizando el modelo de IA Llama y devuelve un análisis estructurado
    /// </summary>
    /// <param name="parkingData">Datos brutos de sensores de aparcamiento en formato JSON string</param>
    /// <param name="parsedData">Datos parseados de múltiples sensores (puede ser null)</param>
    /// <param name="includeViolationAnalysis">Indica si incluir análisis de infracciones de tiempo</param>
    /// <param name="includeSensorHealth">Indica si incluir análisis de estado de sensores</param>
    /// <returns>Objeto ParkingAnalysisResult con el análisis completo o null si hay error</returns>
    Task<ParkingAnalysisResult?> AnalyzeParkingDataAsync(string parkingData, ParkingSensorCollection? parsedData, bool includeViolationAnalysis, bool includeSensorHealth);
}

/// <summary>
/// Implementación del servicio de IA que se conecta a Llama a través de OllamaSharp
/// </summary>
public class LlamaAIService : IAIService
{
    private readonly OllamaApiClient _ollamaClient;
    private readonly string _modelName;

    /// <summary>
    /// Constructor que inicializa el cliente de Ollama con la configuración proporcionada
    /// </summary>
    /// <param name="configuration">Configuración de la aplicación que contiene endpoint y modelo de IA</param>
    public LlamaAIService(IConfiguration configuration)
    {
        // Obtener configuración del endpoint y modelo desde appsettings.json
        var apiEndpoint = configuration.GetValue<string>("LlamaAI:ApiEndpoint") ?? "http://localhost:11434";
        _modelName = configuration.GetValue<string>("LlamaAI:ModelName") ?? "llama3";
        
        // Crear el cliente de Ollama con el endpoint configurado
        _ollamaClient = new OllamaApiClient(new Uri(apiEndpoint))
        {
            SelectedModel = _modelName
        };
    }

    /// <summary>
    /// Método principal que analiza datos meteorológicos usando el modelo Llama
    /// Maneja la comunicación con Ollama, procesa la respuesta JSON y proporciona fallbacks en caso de error
    /// </summary>
    /// <param name="temperatureData">Datos brutos del sensor meteorológico en formato JSON</param>
    /// <param name="parsedData">Objeto TemperatureSensorData parseado (opcional)</param>
    /// <param name="includeSafetyRecommendations">Flag para incluir recomendaciones de seguridad</param>
    /// <param name="includeTrendAnalysis">Flag para incluir análisis de tendencias</param>
    /// <returns>WeatherAnalysisResult con análisis completo o análisis básico en caso de error</returns>
    public async Task<WeatherAnalysisResult?> AnalyzeTemperatureDataAsync(string temperatureData, TemperatureSensorData? parsedData, bool includeSafetyRecommendations, bool includeTrendAnalysis)
    {
        try
        {
            // Crear el prompt específico para el análisis meteorológico
            var prompt = CreateWeatherAnalysisPrompt(temperatureData, parsedData, includeSafetyRecommendations, includeTrendAnalysis);
            
            // Configurar la solicitud para el modelo Llama via OllamaSharp
            var request = new GenerateRequest
            {
                Model = _modelName,
                Prompt = prompt,
                Stream = false // Desactivar streaming para obtener respuesta completa
            };

            // Realizar la llamada asíncrona a Ollama y recopilar la respuesta
            var responseBuilder = new StringBuilder();
            
            await foreach (var responseStream in _ollamaClient.GenerateAsync(request))
            {
                // Acumular fragmentos de respuesta
                if (responseStream?.Response != null)
                {
                    responseBuilder.Append(responseStream.Response);
                }
                
                // Terminar cuando se reciba la señal de finalización
                if (responseStream?.Done == true)
                {
                    break;
                }
            }

            var finalResponse = responseBuilder.ToString();
            
            if (!string.IsNullOrEmpty(finalResponse))
            {
                // Intentar parsear la respuesta JSON del modelo Llama
                return ParseWeatherAnalysisResponse(finalResponse);
            }
            else
            {
                return null;
            }
        }
        catch (HttpRequestException ex)
        {
            // Error de conexión con Ollama - devolver análisis básico como fallback
            return CreateWeatherFallbackAnalysis(parsedData, $"Error de conexión con Ollama: {ex.Message}");
        }
        catch (Exception ex)
        {
            // Error general - devolver análisis básico como fallback
            return CreateWeatherFallbackAnalysis(parsedData, $"Error al conectar con el modelo Llama: {ex.Message}");
        }
    }

    /// <summary>
    /// Analiza datos de aforo/ocupación usando el modelo Llama
    /// Maneja la comunicación con Ollama, procesa la respuesta JSON y proporciona fallbacks en caso de error
    /// </summary>
    /// <param name="occupancyData">Datos brutos del sensor de aforo en formato JSON</param>
    /// <param name="parsedData">Objeto OccupancySensorData parseado (opcional)</param>
    /// <param name="includeFlowAnalysis">Flag para incluir análisis de flujo de personas</param>
    /// <param name="includeAlerts">Flag para incluir alertas y recomendaciones operativas</param>
    /// <returns>OccupancyAnalysisResult con análisis completo o análisis básico en caso de error</returns>
    public async Task<OccupancyAnalysisResult?> AnalyzeOccupancyDataAsync(string occupancyData, OccupancySensorData? parsedData, bool includeFlowAnalysis, bool includeAlerts)
    {
        try
        {
            // Crear el prompt específico para el análisis de aforo
            var prompt = CreateOccupancyAnalysisPrompt(occupancyData, parsedData, includeFlowAnalysis, includeAlerts);
            
            // Configurar y realizar la solicitud a Ollama
            var request = new GenerateRequest
            {
                Model = _modelName,
                Prompt = prompt,
                Stream = false
            };

            var responseBuilder = new StringBuilder();
            
            await foreach (var responseStream in _ollamaClient.GenerateAsync(request))
            {
                if (responseStream?.Response != null)
                {
                    responseBuilder.Append(responseStream.Response);
                }
                
                if (responseStream?.Done == true)
                {
                    break;
                }
            }

            var finalResponse = responseBuilder.ToString();
            
            if (!string.IsNullOrEmpty(finalResponse))
            {
                return ParseOccupancyAnalysisResponse(finalResponse);
            }
            else
            {
                return null;
            }
        }
        catch (HttpRequestException ex)
        {
            return CreateFallbackOccupancyAnalysis(parsedData, $"Error de conexión con Ollama: {ex.Message}");
        }
        catch (Exception ex)
        {
            return CreateFallbackOccupancyAnalysis(parsedData, $"Error al conectar con el modelo Llama: {ex.Message}");
        }
    }

    /// <summary>
    /// Analiza datos de sensores de aparcamiento usando el modelo Llama
    /// Procesa múltiples sensores y proporciona análisis integral de disponibilidad y gestión
    /// </summary>
    /// <param name="parkingData">Datos brutos de sensores de aparcamiento en formato JSON</param>
    /// <param name="parsedData">Colección de datos de sensores parseados (opcional)</param>
    /// <param name="includeViolationAnalysis">Flag para incluir análisis de infracciones</param>
    /// <param name="includeSensorHealth">Flag para incluir análisis de estado de sensores</param>
    /// <returns>ParkingAnalysisResult con análisis completo o análisis básico en caso de error</returns>
    public async Task<ParkingAnalysisResult?> AnalyzeParkingDataAsync(string parkingData, ParkingSensorCollection? parsedData, bool includeViolationAnalysis, bool includeSensorHealth)
    {
        try
        {
            // Crear el prompt específico para el análisis de aparcamiento
            var prompt = CreateParkingAnalysisPrompt(parkingData, parsedData, includeViolationAnalysis, includeSensorHealth);
            
            // Configurar y realizar la solicitud a Ollama
            var request = new GenerateRequest
            {
                Model = _modelName,
                Prompt = prompt,
                Stream = false
            };

            var responseBuilder = new StringBuilder();
            
            await foreach (var responseStream in _ollamaClient.GenerateAsync(request))
            {
                if (responseStream?.Response != null)
                {
                    responseBuilder.Append(responseStream.Response);
                }
                
                if (responseStream?.Done == true)
                {
                    break;
                }
            }

            var finalResponse = responseBuilder.ToString();
            
            if (!string.IsNullOrEmpty(finalResponse))
            {
                return ParseParkingAnalysisResponse(finalResponse);
            }
            else
            {
                return null;
            }
        }
        catch (HttpRequestException ex)
        {
            return CreateFallbackParkingAnalysis(parsedData, $"Error de conexión con Ollama: {ex.Message}");
        }
        catch (Exception ex)
        {
            return CreateFallbackParkingAnalysis(parsedData, $"Error al conectar con el modelo Llama: {ex.Message}");
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
                ComfortDescription = "Análisis básico - IA no disponible",
                ComfortScore = GetBasicComfortScore(parsedData.Temperature)
            };

            fallback.HumidityAnalysis = new HumidityAnalysis
            {
                RelativeHumidity = parsedData.RelativeHumidity,
                HumidityPercentage = parsedData.Humidity,
                HumidityStatus = GetBasicHumidityStatus(parsedData.Humidity),
                HumidityDescription = "Análisis básico - IA no disponible",
                HumidityScore = GetBasicHumidityScore(parsedData.Humidity)
            };

            fallback.WeatherConditions = new WeatherConditionsAnalysis
            {
                WindSpeed = parsedData.WindSpeed,
                WeatherType = parsedData.WeatherType,
                Precipitation = parsedData.Precipitation,
                OverallWeatherStatus = "fair",
                WeatherDescription = $"Análisis básico - {errorMessage}",
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
                StatusDescription = "Análisis básico - IA no disponible",
                EfficiencyScore = GetBasicOccupancyScore(parsedData.OccupancyPercentage)
            };

            if (parsedData.MaxCapacity.HasValue && parsedData.CurrentOccupancy.HasValue)
            {
                fallback.CapacityAnalysis = new CapacityAnalysis
                {
                    AvailableSpaces = Math.Max(0, parsedData.MaxCapacity.Value - parsedData.CurrentOccupancy.Value),
                    AccessRecommendation = GetBasicAccessRecommendation(parsedData.OccupancyPercentage),
                    RecommendationDescription = $"Análisis básico - {errorMessage}"
                };
            }
        }

        return fallback;
    }

    private ParkingAnalysisResult CreateFallbackParkingAnalysis(ParkingSensorCollection? parsedData, string errorMessage)
    {
        var fallback = new ParkingAnalysisResult();

        if (parsedData?.ParkingSpots != null)
        {
            var totalSpots = parsedData.ParkingSpots.Count;
            var occupiedSpots = parsedData.ParkingSpots.Count(s => s.IsOccupied);
            var availableSpots = parsedData.ParkingSpots.Count(s => s.IsAvailable);
            var outOfOrderSpots = parsedData.ParkingSpots.Count(s => s.IsOutOfOrder);
            var occupancyPercentage = totalSpots > 0 ? (double)occupiedSpots / totalSpots * 100 : 0;

            fallback.OccupancyAnalysis = new ParkingOccupancyAnalysis
            {
                TotalSpots = totalSpots,
                OccupiedSpots = occupiedSpots,
                AvailableSpots = availableSpots,
                OutOfOrderSpots = outOfOrderSpots,
                OccupancyPercentage = occupancyPercentage,
                AvailabilityLevel = GetBasicAvailabilityLevel(occupancyPercentage),
                StatusDescription = "Análisis básico - IA no disponible"
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

        var temperatureInfo = parsedData?.Temperature.HasValue == true ? $"- Temperatura: {parsedData.Temperature:F1}°C" : "";
        var humidityInfo = parsedData?.Humidity.HasValue == true ? $"- Humedad: {parsedData.Humidity:F1}%" : "";
        var windInfo = parsedData?.WindSpeed.HasValue == true ? $"- Velocidad del viento: {parsedData.WindSpeed:F1} m/s" : "";
        var weatherInfo = parsedData?.WeatherType != null ? $"- Tipo de clima: {parsedData.WeatherType}" : "";
        var precipitationInfo = parsedData?.Precipitation.HasValue == true ? $"- Precipitación: {parsedData.Precipitation:F1} mm" : "";
        var locationInfo = parsedData?.Location?.Coordinates != null && parsedData.Location.Coordinates.Length >= 2 
            ? $"- Coordenadas: {parsedData.Location.Coordinates[1]:F4}, {parsedData.Location.Coordinates[0]:F4}" : "";

        var prompt = $@"Analiza los siguientes datos de pronóstico meteorológico y responde ÚNICAMENTE con un objeto JSON estructurado.

DATOS DEL SENSOR: {rawData}

ESQUEMA JSON REQUERIDO:
{{""temperatureAnalysis"":{{""currentTemperature"":number,""temperatureFahrenheit"":number,""comfortLevel"":""optimal|acceptable|cold|hot"",""comfortDescription"":""string"",""comfortScore"":number}},""humidityAnalysis"":{{""relativeHumidity"":number,""humidityPercentage"":number,""humidityStatus"":""ideal|dry|humid"",""humidityDescription"":""string"",""humidityScore"":number}},""weatherConditions"":{{""windSpeed"":number,""windCategory"":""calm|light|moderate|strong"",""weatherType"":""string"",""precipitation"":number,""overallWeatherStatus"":""excellent|good|fair|poor"",""weatherDescription"":""string"",""weatherScore"":number}},{safetySection}{trendSection}""activityRecommendations"":{{""outdoorActivities"":[""string1""],""indoorActivities"":[""string1""],""activitiesToAvoid"":[""string1""],""recommendedClothing"":[""string1""],""outdoorSuitabilityScore"":number}}}}

DATOS: {temperatureInfo} {humidityInfo} {windInfo} {weatherInfo} {precipitationInfo} {locationInfo}";

        return prompt;
    }

    private string CreateOccupancyAnalysisPrompt(string rawData, OccupancySensorData? parsedData, bool includeFlowAnalysis, bool includeAlerts)
    {
        var flowSection = includeFlowAnalysis ? @"""trafficPatterns"":{""entrances"":number,""exits"":number,""netFlow"":number,""flowTrend"":""increasing|decreasing|stable"",""shortTermPrediction"":""string""}," : "";
        var alertsSection = includeAlerts ? @"""alerts"":{""alertLevel"":""none|low|medium|high|critical"",""activeAlerts"":[""string1""],""warnings"":[""string1""],""timeToNextAlert"":""string""}," : "";

        var occupancyInfo = parsedData?.CurrentOccupancy.HasValue == true ? $"Ocupación:{parsedData.CurrentOccupancy}" : "";
        var capacityInfo = parsedData?.MaxCapacity.HasValue == true ? $"Capacidad:{parsedData.MaxCapacity}" : "";

        var prompt = $@"Analiza datos de aforo y responde SOLO JSON:

DATOS: {rawData}

ESQUEMA:{{""occupancyAnalysis"":{{""currentOccupancy"":number,""maxCapacity"":number,""occupancyPercentage"":number,""occupancyLevel"":""low|moderate|high|critical|full"",""statusDescription"":""string"",""efficiencyScore"":number}},""capacityAnalysis"":{{""availableSpaces"":number,""accessRecommendation"":""allow|restrict|deny"",""recommendationDescription"":""string""}},{flowSection}{alertsSection}""operationalRecommendations"":{{""immediateActions"":[""string1""],""queueManagement"":[""string1""],""userCommunications"":[""string1""]}}}}

INFO: {occupancyInfo} {capacityInfo}";
        
        return prompt;
    }

    private string CreateParkingAnalysisPrompt(string rawData, ParkingSensorCollection? parsedData, bool includeViolationAnalysis, bool includeSensorHealth)
    {
        var violationSection = includeViolationAnalysis ? @"""violationAnalysis"":{""potentialViolations"":number,""warningSpots"":number,""violationSpots"":[""string1""]}," : "";
        var healthSection = includeSensorHealth ? @"""sensorHealth"":{""totalSensors"":number,""activeSensors"":number,""faultySensors"":number,""systemHealthPercentage"":number}," : "";

        var totalSpots = parsedData?.ParkingSpots?.Count ?? 0;
        var occupiedSpots = parsedData?.ParkingSpots?.Count(s => s.IsOccupied) ?? 0;
        var summaryInfo = totalSpots > 0 ? $"Total:{totalSpots} Ocupadas:{occupiedSpots}" : "";

        var prompt = $@"Analiza datos de aparcamiento y responde SOLO JSON:

DATOS: {rawData}

ESQUEMA:{{""occupancyAnalysis"":{{""totalSpots"":number,""occupiedSpots"":number,""availableSpots"":number,""outOfOrderSpots"":number,""occupancyPercentage"":number,""availabilityLevel"":""high|medium|low|critical"",""statusDescription"":""string""}},{violationSection}{healthSection}""operationalRecommendations"":{{""immediateActions"":[""string1""],""userGuidance"":[""string1""]}},""performanceMetrics"":{{""utilizationEfficiency"":number,""overallPerformanceScore"":number}}}}

INFO: {summaryInfo}";
        
        return prompt;
    }
}