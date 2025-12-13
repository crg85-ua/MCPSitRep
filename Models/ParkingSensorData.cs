using System.Text.Json.Serialization;

/// <summary>
/// Modelo de datos para plazas de aparcamiento según estándar FIWARE dataModel.Parking ParkingSpot
/// </summary>
public class ParkingSensorData
{
    /// <summary>
    /// Unique identifier of the entity
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// NGSI Entity type. It has to be ParkingSpot
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = "ParkingSpot";

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
    /// Owner of the parking spot
    /// </summary>
    [JsonPropertyName("owner")]
    public string[]? Owner { get; set; }

    /// <summary>
    /// A sequence of characters identifying the provider of the harmonized data entity
    /// </summary>
    [JsonPropertyName("source")]
    public string? Source { get; set; }

    /// <summary>
    /// Parking spot name
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Parking spot alternate name
    /// </summary>
    [JsonPropertyName("alternateName")]
    public string? AlternateName { get; set; }

    /// <summary>
    /// A description of this parking spot
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Parking spot number
    /// </summary>
    [JsonPropertyName("number")]
    public string? Number { get; set; }

    /// <summary>
    /// Category of the parking spot. Enum:'onstreet, offstreet'
    /// </summary>
    [JsonPropertyName("category")]
    public string[]? Category { get; set; }

    /// <summary>
    /// The parking site group to which this parking spot belongs to. This attribute shall not be used for representing hierarchical relationships
    /// </summary>
    [JsonPropertyName("refParkingSite")]
    public string? RefParkingSite { get; set; }

    /// <summary>
    /// The parking group to which this parking spot belongs to. This attribute shall not be used for representing hierarchical relationships
    /// </summary>
    [JsonPropertyName("refParkingGroup")]
    public string? RefParkingGroup { get; set; }

    /// <summary>
    /// Parking spot type. Enum:'regular, disabled, resident, women, police, taxi, bus, motorcycle, shortTerm, loading, vehicleHighEmission, vehicleLowEmission, carSharing, carWithTrailer, carWithCaravan, bigCar, smallCar, lorry, withChargingPoint, forEMVehicles'
    /// </summary>
    [JsonPropertyName("spotType")]
    public string? SpotType { get; set; }

    /// <summary>
    /// The width of the parking spot. Units: 'meters'
    /// </summary>
    [JsonPropertyName("width")]
    public double? Width { get; set; }

    /// <summary>
    /// The length of the parking spot. Units: 'meters'
    /// </summary>
    [JsonPropertyName("length")]
    public double? Length { get; set; }

    /// <summary>
    /// The status of the parking spot from the point of view of occupancy. Enum:'occupied, free, closed, unknown'
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>
    /// Timestamp which represents when the parking spot was occupied (when status is occupied) or when it was freed (when status is free)
    /// </summary>
    [JsonPropertyName("timeInstant")]
    public DateTime? TimeInstant { get; set; }

    /// <summary>
    /// If charging point for EV is available at this parking spot. Enum:'yes, no, unknown'
    /// </summary>
    [JsonPropertyName("chargeType")]
    public string[]? ChargeType { get; set; }

    /// <summary>
    /// The address of this parking spot
    /// </summary>
    [JsonPropertyName("address")]
    public Address? Address { get; set; }

    /// <summary>
    /// The geographic location of this parking spot expressed by a GeoJSON geometry
    /// </summary>
    [JsonPropertyName("location")]
    public GeoJsonGeometry? Location { get; set; }

    /// <summary>
    /// The area served by this entity
    /// </summary>
    [JsonPropertyName("areaServed")]
    public string? AreaServed { get; set; }

    /// <summary>
    /// If the parking spot is blue zone or not. Boolean
    /// </summary>
    [JsonPropertyName("blueZone")]
    public bool? BlueZone { get; set; }

    /// <summary>
    /// If the parking spot is green zone or not. Boolean
    /// </summary>
    [JsonPropertyName("greenZone")]
    public bool? GreenZone { get; set; }

    /// <summary>
    /// If this parking spot allows short term stays. Boolean
    /// </summary>
    [JsonPropertyName("shortTerm")]
    public bool? ShortTerm { get; set; }

    /// <summary>
    /// If this parking spot allows medium term stays. Boolean
    /// </summary>
    [JsonPropertyName("mediumTerm")]
    public bool? MediumTerm { get; set; }

    /// <summary>
    /// If this parking spot allows long term stays. Boolean
    /// </summary>
    [JsonPropertyName("longTerm")]
    public bool? LongTerm { get; set; }

    /// <summary>
    /// If this parking spot requires to pay a tariff for using it. Boolean
    /// </summary>
    [JsonPropertyName("requiredPermit")]
    public string[]? RequiredPermit { get; set; }

    // Propiedades adicionales para sensores IoT y gestión avanzada

    /// <summary>
    /// Identificador del sensor de aparcamiento
    /// </summary>
    [JsonPropertyName("sensorId")]
    public string? SensorId { get; set; }

    /// <summary>
    /// Estado del sensor (active, inactive, error, maintenance)
    /// </summary>
    [JsonPropertyName("sensorStatus")]
    public string? SensorStatus { get; set; }

    /// <summary>
    /// Nivel de batería del sensor (0-100)
    /// </summary>
    [JsonPropertyName("batteryLevel")]
    public int? BatteryLevel { get; set; }

    /// <summary>
    /// Tiempo transcurrido desde el último cambio de estado (en minutos)
    /// </summary>
    [JsonPropertyName("timeSinceLastChange")]
    public int? TimeSinceLastChange { get; set; }

    /// <summary>
    /// Zona de aparcamiento (ej: "Zona Azul", "Residentes", "Discapacitados")
    /// </summary>
    [JsonPropertyName("zone")]
    public string? Zone { get; set; }

    /// <summary>
    /// Tarifa por hora (si aplica)
    /// </summary>
    [JsonPropertyName("hourlyRate")]
    public double? HourlyRate { get; set; }

    /// <summary>
    /// Tiempo máximo de estacionamiento permitido (en minutos)
    /// </summary>
    [JsonPropertyName("maxParkingTime")]
    public int? MaxParkingTime { get; set; }

    // Propiedades calculadas para compatibilidad

    /// <summary>
    /// Indica si la plaza está ocupada
    /// </summary>
    [JsonIgnore]
    public bool IsOccupied => Status?.ToLowerInvariant() == "occupied";

    /// <summary>
    /// Indica si la plaza está disponible
    /// </summary>
    [JsonIgnore]
    public bool IsAvailable => Status?.ToLowerInvariant() == "free";

    /// <summary>
    /// Indica si la plaza está fuera de servicio
    /// </summary>
    [JsonIgnore]
    public bool IsOutOfOrder => Status?.ToLowerInvariant() == "closed";

    /// <summary>
    /// Timestamp para compatibilidad
    /// </summary>
    [JsonIgnore]
    public DateTime Timestamp => TimeInstant ?? DateModified ?? DateTime.UtcNow;

    /// <summary>
    /// Descripción de la ubicación
    /// </summary>
    [JsonIgnore]
    public string LocationDescription
    {
        get
        {
            if (!string.IsNullOrEmpty(Number) && !string.IsNullOrEmpty(Zone))
            {
                return $"{Zone} - Plaza {Number}";
            }
            if (!string.IsNullOrEmpty(Number)) return $"Plaza {Number}";
            if (!string.IsNullOrEmpty(Zone)) return Zone;
            if (!string.IsNullOrEmpty(Name)) return Name;

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
    /// Estado de alerta por tiempo de ocupación
    /// </summary>
    [JsonIgnore]
    public string OccupancyAlert
    {
        get
        {
            if (!IsOccupied || !TimeSinceLastChange.HasValue || !MaxParkingTime.HasValue)
                return "Normal";

            var percentage = (double)TimeSinceLastChange.Value / MaxParkingTime.Value * 100;
            return percentage switch
            {
                >= 100 => "Infracción",
                >= 90 => "Crítico",
                >= 75 => "Advertencia",
                _ => "Normal"
            };
        }
    }

    /// <summary>
    /// Número de la plaza para compatibilidad
    /// </summary>
    [JsonIgnore]
    public string? SpotNumber => Number;

    /// <summary>
    /// Estado de ocupación para compatibilidad
    /// </summary>
    [JsonIgnore]
    public string? OccupancyStatus => Status;

    /// <summary>
    /// Fecha y hora para compatibilidad
    /// </summary>
    [JsonIgnore]
    public DateTime DateTime => TimeInstant ?? DateModified ?? DateTime.UtcNow;

    /// <summary>
    /// ID del sensor para compatibilidad
    /// </summary>
    [JsonIgnore]
    public string? Id_Sensor => SensorId;
}

/// <summary>
/// Colección de datos de múltiples sensores de aparcamiento
/// </summary>
public class ParkingSensorCollection
{
    /// <summary>
    /// Lista de plazas de aparcamiento
    /// </summary>
    [JsonPropertyName("parkingSpots")]
    public List<ParkingSensorData> ParkingSpots { get; set; } = new List<ParkingSensorData>();

    /// <summary>
    /// Información de la zona o área de aparcamiento
    /// </summary>
    [JsonPropertyName("areaInfo")]
    public ParkingAreaInfo? AreaInfo { get; set; }

    /// <summary>
    /// Timestamp de la última actualización
    /// </summary>
    [JsonPropertyName("lastUpdate")]
    public DateTime LastUpdate { get; set; }
}

/// <summary>
/// Información del área de aparcamiento
/// </summary>
public class ParkingAreaInfo
{
    /// <summary>
    /// Nombre del área de aparcamiento
    /// </summary>
    [JsonPropertyName("areaName")]
    public string? AreaName { get; set; }

    /// <summary>
    /// Número total de plazas en el área
    /// </summary>
    [JsonPropertyName("totalSpots")]
    public int? TotalSpots { get; set; }

    /// <summary>
    /// Zona o distrito (ej: "Centro", "Zona Universitaria")
    /// </summary>
    [JsonPropertyName("zone")]
    public string? Zone { get; set; }

    /// <summary>
    /// Tipo de aparcamiento (street, garage, lot)
    /// </summary>
    [JsonPropertyName("parkingType")]
    public string? ParkingType { get; set; }
}

/// <summary>
/// Resultado estructurado del análisis de IA de datos de aparcamiento
/// </summary>
public class ParkingAnalysisResult
{
    /// <summary>
    /// Análisis de ocupación general
    /// </summary>
    [JsonPropertyName("occupancyAnalysis")]
    public ParkingOccupancyAnalysis? OccupancyAnalysis { get; set; }

    /// <summary>
    /// Análisis de disponibilidad por zonas
    /// </summary>
    [JsonPropertyName("availabilityAnalysis")]
    public AvailabilityAnalysis? AvailabilityAnalysis { get; set; }

    /// <summary>
    /// Análisis de infracciones y alertas
    /// </summary>
    [JsonPropertyName("violationAnalysis")]
    public ViolationAnalysis? ViolationAnalysis { get; set; }

    /// <summary>
    /// Análisis del estado de sensores
    /// </summary>
    [JsonPropertyName("sensorHealth")]
    public SensorHealthAnalysis? SensorHealth { get; set; }

    /// <summary>
    /// Recomendaciones operativas para gestión de aparcamiento
    /// </summary>
    [JsonPropertyName("operationalRecommendations")]
    public ParkingOperationalRecommendations? OperationalRecommendations { get; set; }

    /// <summary>
    /// Métricas de rendimiento del área
    /// </summary>
    [JsonPropertyName("performanceMetrics")]
    public ParkingPerformanceMetrics? PerformanceMetrics { get; set; }
}

/// <summary>
/// Análisis de ocupación de plazas de aparcamiento
/// </summary>
public class ParkingOccupancyAnalysis
{
    /// <summary>
    /// Número total de plazas monitorizadas
    /// </summary>
    [JsonPropertyName("totalSpots")]
    public int? TotalSpots { get; set; }

    /// <summary>
    /// Número de plazas ocupadas
    /// </summary>
    [JsonPropertyName("occupiedSpots")]
    public int? OccupiedSpots { get; set; }

    /// <summary>
    /// Número de plazas disponibles
    /// </summary>
    [JsonPropertyName("availableSpots")]
    public int? AvailableSpots { get; set; }

    /// <summary>
    /// Número de plazas fuera de servicio
    /// </summary>
    [JsonPropertyName("outOfOrderSpots")]
    public int? OutOfOrderSpots { get; set; }

    /// <summary>
    /// Porcentaje de ocupación
    /// </summary>
    [JsonPropertyName("occupancyPercentage")]
    public double? OccupancyPercentage { get; set; }

    /// <summary>
    /// Nivel de disponibilidad (high, medium, low, critical)
    /// </summary>
    [JsonPropertyName("availabilityLevel")]
    public string? AvailabilityLevel { get; set; }

    /// <summary>
    /// Descripción del estado general
    /// </summary>
    [JsonPropertyName("statusDescription")]
    public string? StatusDescription { get; set; }
}

/// <summary>
/// Análisis de disponibilidad por diferentes categorías
/// </summary>
public class AvailabilityAnalysis
{
    /// <summary>
    /// Disponibilidad por zona
    /// </summary>
    [JsonPropertyName("availabilityByZone")]
    public Dictionary<string, int>? AvailabilityByZone { get; set; }

    /// <summary>
    /// Disponibilidad por tipo de plaza
    /// </summary>
    [JsonPropertyName("availabilityBySpotType")]
    public Dictionary<string, int>? AvailabilityBySpotType { get; set; }

    /// <summary>
    /// Zonas con mayor disponibilidad
    /// </summary>
    [JsonPropertyName("bestAvailabilityZones")]
    public string[]? BestAvailabilityZones { get; set; }

    /// <summary>
    /// Zonas con menor disponibilidad
    /// </summary>
    [JsonPropertyName("criticalAvailabilityZones")]
    public string[]? CriticalAvailabilityZones { get; set; }
}

/// <summary>
/// Análisis de infracciones y alertas de tiempo
/// </summary>
public class ViolationAnalysis
{
    /// <summary>
    /// Número de posibles infracciones detectadas
    /// </summary>
    [JsonPropertyName("potentialViolations")]
    public int? PotentialViolations { get; set; }

    /// <summary>
    /// Plazas en estado de advertencia
    /// </summary>
    [JsonPropertyName("warningSpots")]
    public int? WarningSpots { get; set; }

    /// <summary>
    /// Plazas en estado crítico por tiempo
    /// </summary>
    [JsonPropertyName("criticalTimeSpots")]
    public int? CriticalTimeSpots { get; set; }

    /// <summary>
    /// Lista de plazas con posibles infracciones
    /// </summary>
    [JsonPropertyName("violationSpots")]
    public string[]? ViolationSpots { get; set; }

    /// <summary>
    /// Recomendaciones para control de infracciones
    /// </summary>
    [JsonPropertyName("enforcementRecommendations")]
    public string[]? EnforcementRecommendations { get; set; }
}

/// <summary>
/// Análisis de salud y estado de sensores
/// </summary>
public class SensorHealthAnalysis
{
    /// <summary>
    /// Número total de sensores monitorizados
    /// </summary>
    [JsonPropertyName("totalSensors")]
    public int? TotalSensors { get; set; }

    /// <summary>
    /// Sensores activos y funcionando
    /// </summary>
    [JsonPropertyName("activeSensors")]
    public int? ActiveSensors { get; set; }

    /// <summary>
    /// Sensores con errores o inactivos
    /// </summary>
    [JsonPropertyName("faultySensors")]
    public int? FaultySensors { get; set; }

    /// <summary>
    /// Sensores con batería baja
    /// </summary>
    [JsonPropertyName("lowBatterySensors")]
    public int? LowBatterySensors { get; set; }

    /// <summary>
    /// Porcentaje de salud general del sistema
    /// </summary>
    [JsonPropertyName("systemHealthPercentage")]
    public double? SystemHealthPercentage { get; set; }

    /// <summary>
    /// Sensores que requieren mantenimiento
    /// </summary>
    [JsonPropertyName("maintenanceRequired")]
    public string[]? MaintenanceRequired { get; set; }
}

/// <summary>
/// Recomendaciones operativas para gestión de aparcamiento
/// </summary>
public class ParkingOperationalRecommendations
{
    /// <summary>
    /// Acciones inmediatas recomendadas
    /// </summary>
    [JsonPropertyName("immediateActions")]
    public string[]? ImmediateActions { get; set; }

    /// <summary>
    /// Estrategias para optimizar rotación
    /// </summary>
    [JsonPropertyName("rotationOptimization")]
    public string[]? RotationOptimization { get; set; }

    /// <summary>
    /// Recomendaciones de comunicación para usuarios
    /// </summary>
    [JsonPropertyName("userGuidance")]
    public string[]? UserGuidance { get; set; }

    /// <summary>
    /// Sugerencias de mejora de ingresos
    /// </summary>
    [JsonPropertyName("revenueOptimization")]
    public string[]? RevenueOptimization { get; set; }

    /// <summary>
    /// Prioridades de mantenimiento
    /// </summary>
    [JsonPropertyName("maintenancePriorities")]
    public string[]? MaintenancePriorities { get; set; }
}

/// <summary>
/// Métricas de rendimiento del sistema de aparcamiento
/// </summary>
public class ParkingPerformanceMetrics
{
    /// <summary>
    /// Eficiencia de utilización (0-100)
    /// </summary>
    [JsonPropertyName("utilizationEfficiency")]
    public double? UtilizationEfficiency { get; set; }

    /// <summary>
    /// Tasa de rotación promedio (vehículos por hora)
    /// </summary>
    [JsonPropertyName("averageRotationRate")]
    public double? AverageRotationRate { get; set; }

    /// <summary>
    /// Ingresos estimados por hora
    /// </summary>
    [JsonPropertyName("estimatedHourlyRevenue")]
    public double? EstimatedHourlyRevenue { get; set; }

    /// <summary>
    /// Score de satisfacción del usuario (1-10)
    /// </summary>
    [JsonPropertyName("userSatisfactionScore")]
    public int? UserSatisfactionScore { get; set; }

    /// <summary>
    /// Score general de rendimiento del sistema (1-10)
    /// </summary>
    [JsonPropertyName("overallPerformanceScore")]
    public int? OverallPerformanceScore { get; set; }
}