using System.Text.Json.Serialization;
namespace Parking
{
    
    public class ParkingSensorData
    {
        /// <summary>
        /// Identificador de entidad (NGSI-LD @id)
        /// </summary>
        /// 
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Tipo de entidad (siempre "ParkingSpot" para este modelo)
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = "ParkingSpot";

        /// <summary>
        /// Identificador de sensor asociado con esta plaza de aparcamiento
        /// </summary>
        [JsonPropertyName("id_sensor")]
        public string? Id_sensor { get; set; }

        /// <summary>
        /// Nombre o identificador de la plaza de aparcamiento
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Descripción de la plaza de aparcamiento
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Estado de la plaza de aparcamiento (ocupado, libre, desconocido)
        /// </summary>
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        /// <summary>
        /// Indica si la plaza de aparcamiento es adecuada para personas con movilidad reducida (PMR)
        /// </summary>
        [JsonPropertyName("is_pmr")]
        public bool? Is_pmr { get; set; }

        /// <summary>
        /// Ancho de la plaza de aparcamiento en metros
        /// </summary>
        [JsonPropertyName("width")]
        public double? Width { get; set; }

        /// <summary>
        /// Referencia al sitio de aparcamiento al que pertenece esta plaza (nombre del aparcamiento ubicado en el espacio)
        /// </summary>
        [JsonPropertyName("ref_parking_site")]
        public string? Ref_parking_site { get; set; }

        /// <summary>
        /// Referencia al parking
        /// </summary>
        [JsonPropertyName("ref_parking_group")]
        public string? Ref_parking_group { get; set; }

        /// <summary>
        /// Ubicación geográfica (latitud, longitud)
        /// </summary>
        [JsonPropertyName("location")]
        public Location? Location { get; set; }

        /// <summary>
        /// Marca de tiempo cuando el estado fue modificado por última vez
        /// </summary>
        [JsonPropertyName("date_modified")]
        public DateTime? Date_modified { get; set; }


        /// <summary>
        /// Disponibilidad de punto de carga para vehículos eléctricos
        /// </summary>
        [JsonPropertyName("charging_point")]
        public bool? Charging_point { get; set; }

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
        /// Timestamp de la última actualización
        /// </summary>
        [JsonPropertyName("lastUpdate")]
        public DateTime LastUpdate { get; set; }
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

        /// <summary>
        /// Estado detallado de cada plaza de aparcamiento
        /// </summary>
        [JsonPropertyName("parkingSpots")]
        public List<ParkingSensorData> ParkingSpots { get; set; }


    }

    /// <summary>
    /// Ubicación geográfica según el estándar FIWARE
    /// </summary>
    public class Location
    {
        public double[] Coordinates { get; set; } = new double[2]; // [longitud, latitud]
    }
}