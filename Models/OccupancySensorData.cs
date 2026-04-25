using System.Text.Json.Serialization;
namespace Occupancy
{    
    public class OccupancySensorData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = "PointOfInterest";

        [JsonPropertyName("id_espacio")]
        public string? id_espacio { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("location")]
        public Location? Location { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("subCategory")]
        public string? SubCategory { get; set; }

        [JsonPropertyName("source")]
        public string? Source { get; set; }

        [JsonPropertyName("webSite")]
        public string? WebSite { get; set; }

        [JsonPropertyName("contactPoint")]
        public string? ContactPoint { get; set; }

        [JsonPropertyName("image")]
        public string? Image { get; set; }

        [JsonPropertyName("tags")]
        public string[]? Tags { get; set; }

        [JsonPropertyName("rating")]
        public double? Rating { get; set; }

        [JsonPropertyName("reviewCount")]
        public int? ReviewCount { get; set; }

        [JsonPropertyName("relevance")]
        public double? Relevance { get; set; }

        [JsonPropertyName("municipality")]
        public string? Municipality { get; set; }

        [JsonPropertyName("province")]
        public string? Province { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("openingHours")]
        public string? OpeningHours { get; set; }

        [JsonPropertyName("price")]
        public double? Price { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("freeAccess")]
        public bool? FreeAccess { get; set; }

        [JsonPropertyName("capacity")]
        public int? Capacity { get; set; }

        [JsonPropertyName("occupancy")]
        public int? Occupancy { get; set; }

        [JsonPropertyName("accessForDisabled")]
        public bool? AccessForDisabled { get; set; }

        [JsonPropertyName("availableServices")]
        public string[]? AvailableServices { get; set; }

        [JsonPropertyName("id_sensor")]
        public string? id_sensor { get; set; }

        [JsonPropertyName("beachType")]
        public string? BeachType { get; set; }

        [JsonPropertyName("accessType")]
        public string? AccessType { get; set; }

        [JsonPropertyName("flagStatus")]
        public string? FlagStatus { get; set; }

        [JsonPropertyName("length")]
        public double? Length { get; set; }

        [JsonPropertyName("width")]
        public double? Width { get; set; }

        [JsonPropertyName("dateModified")]
        public DateTime? DateModified { get; set; }

        [JsonIgnore]
        public int? CurrentOccupancy => Occupancy;

        [JsonIgnore]
        public int? MaxCapacity => Capacity;

        [JsonIgnore]
        public double? OccupancyPercentage
        {
            get
            {
                if (Occupancy.HasValue && Capacity.HasValue && Capacity > 0)
                    return Math.Min(100, Math.Max(0, (double)Occupancy.Value / Capacity.Value * 100));
                return null;
            }
        }

        [JsonIgnore]
        public string OccupancyLevel => OccupancyPercentage switch
        {
            null => "Desconocido",
            <= 25 => "Bajo",
            <= 50 => "Moderado",
            <= 75 => "Alto",
            <= 90 => "Muy Alto",
            _ => "Completo"
        };

    }

    /// <summary>
    /// Resultado estructurado del an�lisis de IA de datos de aforo/ocupaci�n
    /// </summary>
    public class OccupancyAnalysisResult
    {
        /// <summary>
        /// Analisis del nivel de ocupacion actual
        /// </summary>
        [JsonPropertyName("occupancyAnalysis")]
        public OccupancyAnalysis? OccupancyAnalysis { get; set; }

        /// <summary>
        /// Analisis de capacidad y flujo
        /// </summary>
        [JsonPropertyName("capacityAnalysis")]
        public CapacityAnalysis? CapacityAnalysis { get; set; }

        /// <summary>
        /// Recomendaciones operativas
        /// </summary>
        [JsonPropertyName("operationalRecommendations")]
        public OperationalRecommendations? OperationalRecommendations { get; set; }

        /// <summary>
        /// Alertas y advertencias
        /// </summary>
        [JsonPropertyName("alerts")]
        public OccupancyAlerts? Alerts { get; set; }
    }

    /// <summary>
    /// Analisis especifico del nivel de ocupacion
    /// </summary>
    public class OccupancyAnalysis
    {
        /// <summary>
        /// Numero actual de ocupantes
        /// </summary>
        [JsonPropertyName("currentOccupancy")]
        public int? CurrentOccupancy { get; set; }

        /// <summary>
        /// Capacidad maxima
        /// </summary>
        [JsonPropertyName("maxCapacity")]
        public int? MaxCapacity { get; set; }

        /// <summary>
        /// Porcentaje de ocupacion actual
        /// </summary>
        [JsonPropertyName("occupancyPercentage")]
        public double? OccupancyPercentage { get; set; }

        /// <summary>
        /// Nivel de ocupacion (low, moderate, high, critical, full)
        /// </summary>
        [JsonPropertyName("occupancyLevel")]
        public string? OccupancyLevel { get; set; }

        /// <summary>
        /// Descripcion del estado de ocupacion
        /// </summary>
        [JsonPropertyName("statusDescription")]
        public string? StatusDescription { get; set; }

        /// <summary>
        /// Score de eficiencia del espacio (1-10)
        /// </summary>
        [JsonPropertyName("efficiencyScore")]
        public double? EfficiencyScore { get; set; }
    }

    /// <summary>
    /// Analisis de capacidad y gestion de flujo
    /// </summary>
    public class CapacityAnalysis
    {
        /// <summary>
        /// Espacios disponibles restantes
        /// </summary>
        [JsonPropertyName("availableSpaces")]
        public int? AvailableSpaces { get; set; }

        /// <summary>
        /// Recomendacion de acceso (allow, restrict, deny)
        /// </summary>
        [JsonPropertyName("accessRecommendation")]
        public string? AccessRecommendation { get; set; }

        /// <summary>
        /// Descripcion de la recomendacion
        /// </summary>
        [JsonPropertyName("recommendationDescription")]
        public string? RecommendationDescription { get; set; }
    }

    /// <summary>
    /// Recomendaciones operativas
    /// </summary>
    public class OperationalRecommendations
    {
        /// <summary>
        /// Acciones inmediatas recomendadas
        /// </summary>
        [JsonPropertyName("immediateActions")]
        public string[]? ImmediateActions { get; set; }

        /// <summary>
        /// Estrategias de gestion de cola
        /// </summary>
        [JsonPropertyName("queueManagement")]
        public string[]? QueueManagement { get; set; }

        /// <summary>
        /// Comunicaciones sugeridas para usuarios
        /// </summary>
        [JsonPropertyName("userCommunications")]
        public string[]? UserCommunications { get; set; }

    }

    /// <summary>
    /// Alertas y advertencias del sistema
    /// </summary>
    public class OccupancyAlerts
    {
        /// <summary>
        /// Nivel de alerta general (none, low, medium, high, critical)
        /// </summary>
        [JsonPropertyName("alertLevel")]
        public string? AlertLevel { get; set; }

        /// <summary>
        /// Alertas activas
        /// </summary>
        [JsonPropertyName("activeAlerts")]
        public string[]? ActiveAlerts { get; set; }

        /// <summary>
        /// Advertencias preventivas
        /// </summary>
        [JsonPropertyName("warnings")]
        public string[]? Warnings { get; set; }

        /// <summary>
        /// Tiempo estimado hasta el proximo nivel de alerta
        /// </summary>
        [JsonPropertyName("timeToNextAlert")]
        public string? TimeToNextAlert { get; set; }
    }

    public class Location
    {
        public double[] Coordinates { get; set; } = new double[2]; // [longitud, latitud]
    }
}
