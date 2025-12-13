using System.Text.Json.Serialization;

/// <summary>
/// Modelo de datos para sensores de aforo basado en estándar FIWARE dataModel.PointOfInterest Beach
/// Adaptado para representar espacios con capacidad de aforo y control de ocupación
/// </summary>
public class OccupancySensorData
{
    /// <summary>
    /// Unique identifier of the entity
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// NGSI Entity type. It has to be Beach (adapted for occupancy monitoring)
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = "Beach";

    /// <summary>
    /// A sequence of characters giving the original source of the entity data as a URL
    /// </summary>
    [JsonPropertyName("dataProvider")]
    public string? DataProvider { get; set; }

    /// <summary>
    /// Timestamp when the entity was created
    /// </summary>
    [JsonPropertyName("dateCreated")]
    public DateTime? DateCreated { get; set; }

    /// <summary>
    /// Timestamp when the entity was last modified
    /// </summary>
    [JsonPropertyName("dateModified")]
    public DateTime? DateModified { get; set; }

    /// <summary>
    /// Owner of this Point of Interest
    /// </summary>
    [JsonPropertyName("owner")]
    public string[]? Owner { get; set; }

    /// <summary>
    /// A sequence of characters identifying the provider of the harmonized data entity
    /// </summary>
    [JsonPropertyName("source")]
    public string? Source { get; set; }

    /// <summary>
    /// The name of this Point of Interest
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// An alternative name for this Point of Interest
    /// </summary>
    [JsonPropertyName("alternateName")]
    public string? AlternateName { get; set; }

    /// <summary>
    /// A description of this Point of Interest
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Category of this Point of Interest. Enum: 'public, private, commercialArea, industrialArea, residentialArea, religiousArea, emergencyArea'
    /// </summary>
    [JsonPropertyName("category")]
    public string[]? Category { get; set; }

    /// <summary>
    /// The address of this Point of Interest
    /// </summary>
    [JsonPropertyName("address")]
    public Address? Address { get; set; }

    /// <summary>
    /// The geographic location of this Point of Interest
    /// </summary>
    [JsonPropertyName("location")]
    public GeoJsonGeometry? Location { get; set; }

    /// <summary>
    /// The area served by this entity
    /// </summary>
    [JsonPropertyName("areaServed")]
    public string? AreaServed { get; set; }

    /// <summary>
    /// Beach width. The unit code (text) of measurement given using the UN/CEFACT Common Codes. For instance, MTR represents Metre
    /// </summary>
    [JsonPropertyName("width")]
    public double? Width { get; set; }

    /// <summary>
    /// Beach length. The unit code (text) of measurement given using the UN/CEFACT Common Codes. For instance, MTR represents Metre
    /// </summary>
    [JsonPropertyName("length")]
    public double? Length { get; set; }

    /// <summary>
    /// Total beach occupancy during this observation
    /// </summary>
    [JsonPropertyName("occupancy")]
    public int? Occupancy { get; set; }

    /// <summary>
    /// People present in the area during this observation
    /// </summary>
    [JsonPropertyName("peopleOccupancy")]
    public int? PeopleOccupancy { get; set; }

    /// <summary>
    /// Maximum allowed capacity for the area
    /// </summary>
    [JsonPropertyName("refPointOfInterest")]
    public string? RefPointOfInterest { get; set; }

    /// <summary>
    /// It specifies the facilities provided at the beach. Enum: 'promenade, showers, cleaningServices, lifeGuard, sunshadeRental, sunLoungerRental, waterCraftRental, toilets, touristOffice, litterBins, telephone, surfPracticeArea, accessforDisabled'
    /// </summary>
    [JsonPropertyName("facilities")]
    public string[]? Facilities { get; set; }

    /// <summary>
    /// It specifies the type of available access to the beach. Enum: 'privateVehicle, bus, metro, walk, bike, boat'
    /// </summary>
    [JsonPropertyName("accessType")]
    public string[]? AccessType { get; set; }

    /// <summary>
    /// Last update timestamp for the occupancy data
    /// </summary>
    [JsonPropertyName("dateObserved")]
    public DateTime? DateObserved { get; set; }

    // Propiedades específicas para sensores de aforo y ocupación

    /// <summary>
    /// Identificador del sensor de aforo
    /// </summary>
    [JsonPropertyName("sensorId")]
    public string? SensorId { get; set; }

    /// <summary>
    /// Capacidad máxima del espacio
    /// </summary>
    [JsonPropertyName("maximumCapacity")]
    public int? MaximumCapacity { get; set; }

    /// <summary>
    /// Número de entradas detectadas en el período
    /// </summary>
    [JsonPropertyName("entrances")]
    public int? Entrances { get; set; }

    /// <summary>
    /// Número de salidas detectadas en el período
    /// </summary>
    [JsonPropertyName("exits")]
    public int? Exits { get; set; }

    /// <summary>
    /// Estado del sensor (active, inactive, error, maintenance)
    /// </summary>
    [JsonPropertyName("sensorStatus")]
    public string? SensorStatus { get; set; }

    /// <summary>
    /// Zona o área específica (ej: "Zona A", "Planta 1", "Sector Norte")
    /// </summary>
    [JsonPropertyName("zone")]
    public string? Zone { get; set; }

    /// <summary>
    /// Estado de ocupación del área. Enum: 'available, occupied, full, closed'
    /// </summary>
    [JsonPropertyName("occupancyStatus")]
    public string? OccupancyStatus { get; set; }

    // Propiedades calculadas para compatibilidad

    /// <summary>
    /// Número de personas actualmente detectadas (alias para peopleOccupancy)
    /// </summary>
    [JsonIgnore]
    public int? CurrentOccupancy => PeopleOccupancy ?? Occupancy;

    /// <summary>
    /// Capacidad máxima del espacio (alias para maximumCapacity)
    /// </summary>
    [JsonIgnore]
    public int? MaxCapacity => MaximumCapacity;

    /// <summary>
    /// Porcentaje de ocupación calculado (0-100)
    /// </summary>
    [JsonIgnore]
    public double? OccupancyPercentage 
    { 
        get 
        {
            var current = CurrentOccupancy;
            var max = MaxCapacity;
            
            if (current.HasValue && max.HasValue && max > 0)
            {
                return Math.Min(100, Math.Max(0, (double)current.Value / max.Value * 100));
            }
            return null;
        }
    }

    /// <summary>
    /// Descripción textual del nivel de ocupación
    /// </summary>
    [JsonIgnore]
    public string OccupancyLevel
    {
        get
        {
            if (!OccupancyPercentage.HasValue) return "Desconocido";
            
            return OccupancyPercentage.Value switch
            {
                <= 25 => "Bajo",
                <= 50 => "Moderado",
                <= 75 => "Alto",
                <= 90 => "Muy Alto",
                _ => "Completo"
            };
        }
    }

    /// <summary>
    /// Timestamp para compatibilidad con código existente
    /// </summary>
    [JsonIgnore]
    public DateTime Timestamp => DateObserved ?? DateModified ?? DateTime.UtcNow;

    /// <summary>
    /// Descripción de la ubicación
    /// </summary>
    [JsonIgnore]
    public string LocationDescription
    {
        get
        {
            if (!string.IsNullOrEmpty(Name)) return Name;
            if (!string.IsNullOrEmpty(Zone)) return Zone;
            
            if (Address != null)
            {
                var parts = new List<string>();
                if (!string.IsNullOrEmpty(Address.StreetAddress)) parts.Add(Address.StreetAddress);
                if (!string.IsNullOrEmpty(Address.AddressLocality)) parts.Add(Address.AddressLocality);
                if (parts.Any()) return string.Join(", ", parts);
            }

            if (Location?.Type == "Point" && Location.Coordinates?.Length >= 2)
            {
                return $"Lat: {Location.Coordinates[1]:F4}, Lon: {Location.Coordinates[0]:F4}";
            }
            
            return !string.IsNullOrEmpty(SensorId) ? $"Sensor: {SensorId}" : "Ubicación desconocida";
        }
    }

    /// <summary>
    /// Fecha y hora para compatibilidad
    /// </summary>
    [JsonIgnore]
    public DateTime DateTime => DateObserved ?? DateModified ?? DateTime.UtcNow;

    /// <summary>
    /// ID del sensor para compatibilidad
    /// </summary>
    [JsonIgnore]
    public string? Id_Sensor => SensorId;

    /// <summary>
    /// Estado del sensor para compatibilidad
    /// </summary>
    [JsonIgnore]
    public string? Status => SensorStatus;
}

/// <summary>
/// Resultado estructurado del análisis de IA de datos de aforo/ocupación
/// </summary>
public class OccupancyAnalysisResult
{
    /// <summary>
    /// Análisis del nivel de ocupación actual
    /// </summary>
    [JsonPropertyName("occupancyAnalysis")]
    public OccupancyAnalysis? OccupancyAnalysis { get; set; }

    /// <summary>
    /// Análisis de capacidad y flujo
    /// </summary>
    [JsonPropertyName("capacityAnalysis")]
    public CapacityAnalysis? CapacityAnalysis { get; set; }

    /// <summary>
    /// Análisis de patrones de tráfico
    /// </summary>
    [JsonPropertyName("trafficPatterns")]
    public TrafficPatterns? TrafficPatterns { get; set; }

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
/// Análisis específico del nivel de ocupación
/// </summary>
public class OccupancyAnalysis
{
    /// <summary>
    /// Número actual de ocupantes
    /// </summary>
    [JsonPropertyName("currentOccupancy")]
    public int? CurrentOccupancy { get; set; }

    /// <summary>
    /// Capacidad máxima
    /// </summary>
    [JsonPropertyName("maxCapacity")]
    public int? MaxCapacity { get; set; }

    /// <summary>
    /// Porcentaje de ocupación actual
    /// </summary>
    [JsonPropertyName("occupancyPercentage")]
    public double? OccupancyPercentage { get; set; }

    /// <summary>
    /// Nivel de ocupación (low, moderate, high, critical, full)
    /// </summary>
    [JsonPropertyName("occupancyLevel")]
    public string? OccupancyLevel { get; set; }

    /// <summary>
    /// Descripción del estado de ocupación
    /// </summary>
    [JsonPropertyName("statusDescription")]
    public string? StatusDescription { get; set; }

    /// <summary>
    /// Score de eficiencia del espacio (1-10)
    /// </summary>
    [JsonPropertyName("efficiencyScore")]
    public int? EfficiencyScore { get; set; }
}

/// <summary>
/// Análisis de capacidad y gestión de flujo
/// </summary>
public class CapacityAnalysis
{
    /// <summary>
    /// Espacios disponibles restantes
    /// </summary>
    [JsonPropertyName("availableSpaces")]
    public int? AvailableSpaces { get; set; }

    /// <summary>
    /// Tiempo estimado hasta capacidad completa
    /// </summary>
    [JsonPropertyName("timeToFullCapacity")]
    public string? TimeToFullCapacity { get; set; }

    /// <summary>
    /// Recomendación de acceso (allow, restrict, deny)
    /// </summary>
    [JsonPropertyName("accessRecommendation")]
    public string? AccessRecommendation { get; set; }

    /// <summary>
    /// Descripción de la recomendación
    /// </summary>
    [JsonPropertyName("recommendationDescription")]
    public string? RecommendationDescription { get; set; }
}

/// <summary>
/// Análisis de patrones de tráfico
/// </summary>
public class TrafficPatterns
{
    /// <summary>
    /// Entradas en el período actual
    /// </summary>
    [JsonPropertyName("entrances")]
    public int? Entrances { get; set; }

    /// <summary>
    /// Salidas en el período actual
    /// </summary>
    [JsonPropertyName("exits")]
    public int? Exits { get; set; }

    /// <summary>
    /// Flujo neto (entradas - salidas)
    /// </summary>
    [JsonPropertyName("netFlow")]
    public int? NetFlow { get; set; }

    /// <summary>
    /// Tendencia de flujo (increasing, decreasing, stable)
    /// </summary>
    [JsonPropertyName("flowTrend")]
    public string? FlowTrend { get; set; }

    /// <summary>
    /// Predicción de ocupación a corto plazo
    /// </summary>
    [JsonPropertyName("shortTermPrediction")]
    public string? ShortTermPrediction { get; set; }
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
    /// Estrategias de gestión de cola
    /// </summary>
    [JsonPropertyName("queueManagement")]
    public string[]? QueueManagement { get; set; }

    /// <summary>
    /// Comunicaciones sugeridas para usuarios
    /// </summary>
    [JsonPropertyName("userCommunications")]
    public string[]? UserCommunications { get; set; }

    /// <summary>
    /// Optimizaciones sugeridas
    /// </summary>
    [JsonPropertyName("optimizations")]
    public string[]? Optimizations { get; set; }
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
    /// Tiempo estimado hasta el próximo nivel de alerta
    /// </summary>
    [JsonPropertyName("timeToNextAlert")]
    public string? TimeToNextAlert { get; set; }
}