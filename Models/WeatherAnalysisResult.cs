using System.Text.Json.Serialization;

/// <summary>
/// Resultado estructurado del análisis de IA de datos meteorológicos
/// </summary>
public class WeatherAnalysisResult
{
    /// <summary>
    /// Análisis de la temperatura
    /// </summary>
    [JsonPropertyName("temperatureAnalysis")]
    public TemperatureAnalysis? TemperatureAnalysis { get; set; }

    /// <summary>
    /// Análisis de la humedad
    /// </summary>
    [JsonPropertyName("humidityAnalysis")]
    public HumidityAnalysis? HumidityAnalysis { get; set; }

    /// <summary>
    /// Análisis de las condiciones meteorológicas generales
    /// </summary>
    [JsonPropertyName("weatherConditions")]
    public WeatherConditionsAnalysis? WeatherConditions { get; set; }

    /// <summary>
    /// Análisis de la ubicación
    /// </summary>
    [JsonPropertyName("locationAnalysis")]
    public LocationAnalysis? LocationAnalysis { get; set; }

    /// <summary>
    /// Recomendaciones de seguridad y salud
    /// </summary>
    [JsonPropertyName("safetyRecommendations")]
    public SafetyRecommendations? SafetyRecommendations { get; set; }

    /// <summary>
    /// Análisis de tendencias
    /// </summary>
    [JsonPropertyName("trendAnalysis")]
    public TrendAnalysis? TrendAnalysis { get; set; }

    /// <summary>
    /// Recomendaciones de actividades
    /// </summary>
    [JsonPropertyName("activityRecommendations")]
    public ActivityRecommendations? ActivityRecommendations { get; set; }
}

/// <summary>
/// Análisis específico de temperatura
/// </summary>
public class TemperatureAnalysis
{
    /// <summary>
    /// Temperatura actual en Celsius
    /// </summary>
    [JsonPropertyName("currentTemperature")]
    public double? CurrentTemperature { get; set; }

    /// <summary>
    /// Temperatura en Fahrenheit
    /// </summary>
    [JsonPropertyName("temperatureFahrenheit")]
    public double? TemperatureFahrenheit { get; set; }

    /// <summary>
    /// Nivel de confort (optimal, acceptable, cold, hot)
    /// </summary>
    [JsonPropertyName("comfortLevel")]
    public string? ComfortLevel { get; set; }

    /// <summary>
    /// Descripción del estado de confort
    /// </summary>
    [JsonPropertyName("comfortDescription")]
    public string? ComfortDescription { get; set; }

    /// <summary>
    /// Puntuación de confort del 1-10
    /// </summary>
    [JsonPropertyName("comfortScore")]
    public int? ComfortScore { get; set; }
}

/// <summary>
/// Análisis específico de humedad
/// </summary>
public class HumidityAnalysis
{
    /// <summary>
    /// Humedad relativa actual (0-1)
    /// </summary>
    [JsonPropertyName("relativeHumidity")]
    public double? RelativeHumidity { get; set; }

    /// <summary>
    /// Humedad en porcentaje
    /// </summary>
    [JsonPropertyName("humidityPercentage")]
    public double? HumidityPercentage { get; set; }

    /// <summary>
    /// Estado de la humedad (ideal, dry, humid)
    /// </summary>
    [JsonPropertyName("humidityStatus")]
    public string? HumidityStatus { get; set; }

    /// <summary>
    /// Descripción del estado de humedad
    /// </summary>
    [JsonPropertyName("humidityDescription")]
    public string? HumidityDescription { get; set; }

    /// <summary>
    /// Puntuación de humedad del 1-10
    /// </summary>
    [JsonPropertyName("humidityScore")]
    public int? HumidityScore { get; set; }
}

/// <summary>
/// Análisis de condiciones meteorológicas generales
/// </summary>
public class WeatherConditionsAnalysis
{
    /// <summary>
    /// Velocidad del viento en m/s
    /// </summary>
    [JsonPropertyName("windSpeed")]
    public double? WindSpeed { get; set; }

    /// <summary>
    /// Categoría del viento (calm, light, moderate, strong)
    /// </summary>
    [JsonPropertyName("windCategory")]
    public string? WindCategory { get; set; }

    /// <summary>
    /// Tipo de clima
    /// </summary>
    [JsonPropertyName("weatherType")]
    public string? WeatherType { get; set; }

    /// <summary>
    /// Precipitación en mm
    /// </summary>
    [JsonPropertyName("precipitation")]
    public double? Precipitation { get; set; }

    /// <summary>
    /// Estado general del clima (excellent, good, fair, poor)
    /// </summary>
    [JsonPropertyName("overallWeatherStatus")]
    public string? OverallWeatherStatus { get; set; }

    /// <summary>
    /// Descripción general del clima
    /// </summary>
    [JsonPropertyName("weatherDescription")]
    public string? WeatherDescription { get; set; }

    /// <summary>
    /// Puntuación general del clima del 1-10
    /// </summary>
    [JsonPropertyName("weatherScore")]
    public int? WeatherScore { get; set; }
}

/// <summary>
/// Análisis de ubicación
/// </summary>
public class LocationAnalysis
{
    /// <summary>
    /// Latitud
    /// </summary>
    [JsonPropertyName("latitude")]
    public double? Latitude { get; set; }

    /// <summary>
    /// Longitud
    /// </summary>
    [JsonPropertyName("longitude")]
    public double? Longitude { get; set; }

    /// <summary>
    /// Descripción de la ubicación
    /// </summary>
    [JsonPropertyName("locationDescription")]
    public string? LocationDescription { get; set; }

    /// <summary>
    /// Zona climática estimada
    /// </summary>
    [JsonPropertyName("climateZone")]
    public string? ClimateZone { get; set; }

    /// <summary>
    /// Consideraciones específicas de la ubicación
    /// </summary>
    [JsonPropertyName("locationConsiderations")]
    public string[]? LocationConsiderations { get; set; }
}

/// <summary>
/// Recomendaciones de seguridad y salud
/// </summary>
public class SafetyRecommendations
{
    /// <summary>
    /// Nivel de riesgo general (low, moderate, high, extreme)
    /// </summary>
    [JsonPropertyName("riskLevel")]
    public string? RiskLevel { get; set; }

    /// <summary>
    /// Alertas específicas
    /// </summary>
    [JsonPropertyName("alerts")]
    public string[]? Alerts { get; set; }

    /// <summary>
    /// Acciones recomendadas
    /// </summary>
    [JsonPropertyName("recommendedActions")]
    public string[]? RecommendedActions { get; set; }

    /// <summary>
    /// Consideraciones de salud
    /// </summary>
    [JsonPropertyName("healthConsiderations")]
    public string[]? HealthConsiderations { get; set; }
}

/// <summary>
/// Análisis de tendencias
/// </summary>
public class TrendAnalysis
{
    /// <summary>
    /// Tendencia de temperatura (rising, falling, stable)
    /// </summary>
    [JsonPropertyName("temperatureTrend")]
    public string? TemperatureTrend { get; set; }

    /// <summary>
    /// Pronóstico a corto plazo
    /// </summary>
    [JsonPropertyName("shortTermForecast")]
    public string? ShortTermForecast { get; set; }

    /// <summary>
    /// Patrones identificados
    /// </summary>
    [JsonPropertyName("identifiedPatterns")]
    public string[]? IdentifiedPatterns { get; set; }

    /// <summary>
    /// Recomendaciones de monitoreo
    /// </summary>
    [JsonPropertyName("monitoringRecommendations")]
    public string[]? MonitoringRecommendations { get; set; }
}

/// <summary>
/// Recomendaciones de actividades
/// </summary>
public class ActivityRecommendations
{
    /// <summary>
    /// Actividades al aire libre recomendadas
    /// </summary>
    [JsonPropertyName("outdoorActivities")]
    public string[]? OutdoorActivities { get; set; }

    /// <summary>
    /// Actividades en interiores recomendadas
    /// </summary>
    [JsonPropertyName("indoorActivities")]
    public string[]? IndoorActivities { get; set; }

    /// <summary>
    /// Actividades a evitar
    /// </summary>
    [JsonPropertyName("activitiesToAvoid")]
    public string[]? ActivitiesToAvoid { get; set; }

    /// <summary>
    /// Vestimenta recomendada
    /// </summary>
    [JsonPropertyName("recommendedClothing")]
    public string[]? RecommendedClothing { get; set; }

    /// <summary>
    /// Nivel de adecuación para actividades al aire libre (1-10)
    /// </summary>
    [JsonPropertyName("outdoorSuitabilityScore")]
    public int? OutdoorSuitabilityScore { get; set; }
}