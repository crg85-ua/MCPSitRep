using System.Text.Json.Serialization;
namespace Weather
{
    
    /// <summary>
    /// Resultado estructurado del an�lisis de IA de datos meteorol�gicos
    /// </summary>
    public class WeatherAnalysisResult
    {
        /// <summary>
        /// An�lisis de la temperatura
        /// </summary>
        [JsonPropertyName("temperatureAnalysis")]
        public TemperatureAnalysis? TemperatureAnalysis { get; set; }

        /// <summary>
        /// An�lisis de la humedad
        /// </summary>
        [JsonPropertyName("humidityAnalysis")]
        public HumidityAnalysis? HumidityAnalysis { get; set; }

        /// <summary>
        /// An�lisis de las condiciones meteorol�gicas generales
        /// </summary>
        [JsonPropertyName("weatherConditions")]
        public WeatherConditionsAnalysis? WeatherConditions { get; set; }

        /// <summary>
        /// Recomendaciones de seguridad y salud
        /// </summary>
        [JsonPropertyName("safetyRecommendations")]
        public SafetyRecommendations? SafetyRecommendations { get; set; }

        /// <summary>
        /// An�lisis de tendencias
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
    /// An�lisis espec�fico de temperatura
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
        /// Descripci�n del estado de confort
        /// </summary>
        [JsonPropertyName("comfortDescription")]
        public string? ComfortDescription { get; set; }

        /// <summary>
        /// Puntuaci�n de confort del 1-10
        /// </summary>
        [JsonPropertyName("comfortScore")]
        public double? ComfortScore { get; set; }
    }

    /// <summary>
    /// An�lisis espec�fico de humedad
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
        /// Descripci�n del estado de humedad
        /// </summary>
        [JsonPropertyName("humidityDescription")]
        public string? HumidityDescription { get; set; }

        /// <summary>
        /// Puntuaci�n de humedad del 1-10
        /// </summary>
        [JsonPropertyName("humidityScore")]
        public double? HumidityScore { get; set; }
    }

    /// <summary>
    /// An�lisis de condiciones meteorol�gicas generales
    /// </summary>
    public class WeatherConditionsAnalysis
    {
        /// <summary>
        /// Velocidad del viento en m/s
        /// </summary>
        [JsonPropertyName("windSpeed")]
        public double? WindSpeed { get; set; }

        /// <summary>
        /// Categor�a del viento (calm, light, moderate, strong)
        /// </summary>
        [JsonPropertyName("windCategory")]
        public string? WindCategory { get; set; }

        /// <summary>
        /// Tipo de clima
        /// </summary>
        [JsonPropertyName("weatherType")]
        public string? WeatherType { get; set; }

        /// <summary>
        /// Precipitaci�n en mm
        /// </summary>
        [JsonPropertyName("precipitation")]
        public double? Precipitation { get; set; }

        /// <summary>
        /// Estado general del clima (excellent, good, fair, poor)
        /// </summary>
        [JsonPropertyName("overallWeatherStatus")]
        public string? OverallWeatherStatus { get; set; }

        /// <summary>
        /// Descripci�n general del clima
        /// </summary>
        [JsonPropertyName("weatherDescription")]
        public string? WeatherDescription { get; set; }

        /// <summary>
        /// Puntuaci�n general del clima del 1-10
        /// </summary>
        [JsonPropertyName("weatherScore")]
        public double? WeatherScore { get; set; }
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
        /// Alertas espec�ficas
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
    /// An�lisis de tendencias
    /// </summary>
    public class TrendAnalysis
    {
        /// <summary>
        /// Tendencia de temperatura (rising, falling, stable)
        /// </summary>
        [JsonPropertyName("temperatureTrend")]
        public string? TemperatureTrend { get; set; }

        /// <summary>
        /// Pron�stico a corto plazo
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
        /// Nivel de adecuaci�n para actividades al aire libre (1-10)
        /// </summary>
        [JsonPropertyName("outdoorSuitabilityScore")]
        public double? OutdoorSuitabilityScore { get; set; }
    }
}
