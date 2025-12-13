using System.Text.Json.Serialization;

/// <summary>
/// Modelo de datos para pronóstico meteorológico según estándar FIWARE dataModel.Weather WeatherForecast
/// </summary>
public class TemperatureSensorData
{
    /// <summary>
    /// Unique identifier of the entity
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Entity type. It must be equal to WeatherForecast
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = "WeatherForecast";

    /// <summary>
    /// A sequence of characters giving the original source of the entity data as a URL
    /// </summary>
    [JsonPropertyName("dataProvider")]
    public string? DataProvider { get; set; }

    /// <summary>
    /// Creation date of the entity
    /// </summary>
    [JsonPropertyName("dateCreated")]
    public DateTime? DateCreated { get; set; }

    /// <summary>
    /// Last modification date of the entity
    /// </summary>
    [JsonPropertyName("dateModified")]
    public DateTime? DateModified { get; set; }

    /// <summary>
    /// The date and time the forecast was issued by the meteorological bureau in ISO8601 UTC format
    /// </summary>
    [JsonPropertyName("dateIssued")]
    public DateTime? DateIssued { get; set; }

    /// <summary>
    /// The date and time of the beginning of the validity period for this forecast in ISO8601 UTC format
    /// </summary>
    [JsonPropertyName("validFrom")]
    public DateTime? ValidFrom { get; set; }

    /// <summary>
    /// The date and time of the end of the validity period for this forecast in ISO8601 UTC format
    /// </summary>
    [JsonPropertyName("validTo")]
    public DateTime? ValidTo { get; set; }

    /// <summary>
    /// The date and time for which this forecast refers to in ISO8601 UTC format
    /// </summary>
    [JsonPropertyName("dateRetrieved")]
    public DateTime? DateRetrieved { get; set; }

    /// <summary>
    /// The forecasted value for the temperature. Units: 'celsius degrees'
    /// </summary>
    [JsonPropertyName("temperature")]
    public double? Temperature { get; set; }

    /// <summary>
    /// The forecasted value for the air temperature felt. Units: 'celsius degrees'
    /// </summary>
    [JsonPropertyName("feelsLikeTemperature")]
    public double? FeelsLikeTemperature { get; set; }

    /// <summary>
    /// The forecasted value for the relative humidity. Units: 'percent'
    /// </summary>
    [JsonPropertyName("relativeHumidity")]
    public double? RelativeHumidity { get; set; }

    /// <summary>
    /// The forecasted value for the atmospheric pressure. Units: 'hecto pascals'
    /// </summary>
    [JsonPropertyName("atmosphericPressure")]
    public double? AtmosphericPressure { get; set; }

    /// <summary>
    /// The forecasted value for the wind speed. Units: 'meter per second'
    /// </summary>
    [JsonPropertyName("windSpeed")]
    public double? WindSpeed { get; set; }

    /// <summary>
    /// The forecasted value for the wind direction. Units: 'degrees'
    /// </summary>
    [JsonPropertyName("windDirection")]
    public double? WindDirection { get; set; }

    /// <summary>
    /// The forecasted UV index. Units: 'UV Index'
    /// </summary>
    [JsonPropertyName("uvIndexMax")]
    public int? UvIndexMax { get; set; }

    /// <summary>
    /// The forecasted value for the precipitation accumulated over a measurement period. Units: 'millimeter'
    /// </summary>
    [JsonPropertyName("precipitation")]
    public double? Precipitation { get; set; }

    /// <summary>
    /// The forecasted value for the probability of precipitation. Units: 'percent'
    /// </summary>
    [JsonPropertyName("precipitationProbability")]
    public double? PrecipitationProbability { get; set; }

    /// <summary>
    /// The forecasted value for the visibility. Units: 'kilometer'
    /// </summary>
    [JsonPropertyName("visibility")]
    public double? Visibility { get; set; }

    /// <summary>
    /// Text description of the weather
    /// </summary>
    [JsonPropertyName("weatherType")]
    public string? WeatherType { get; set; }

    /// <summary>
    /// The geo-location of this forecast expressed by a GeoJSON geometry
    /// </summary>
    [JsonPropertyName("location")]
    public GeoJsonGeometry? Location { get; set; }

    /// <summary>
    /// The address of this forecast
    /// </summary>
    [JsonPropertyName("address")]
    public Address? Address { get; set; }

    /// <summary>
    /// A locality within the administrative area
    /// </summary>
    [JsonPropertyName("areaServed")]
    public string? AreaServed { get; set; }

    /// <summary>
    /// A source URL that can be used to fetch the weather forecast
    /// </summary>
    [JsonPropertyName("source")]
    public string? Source { get; set; }

    /// <summary>
    /// Categorization of this forecast by its geographic area
    /// </summary>
    [JsonPropertyName("category")]
    public string[]? Category { get; set; }

    /// <summary>
    /// Day of week from this forecast. 0=Monday, 1=Tuesday, ..., 6=Sunday
    /// </summary>
    [JsonPropertyName("dayOfWeek")]
    public int? DayOfWeek { get; set; }

    /// <summary>
    /// Moment (hour) of day from this forecast
    /// </summary>
    [JsonPropertyName("dayMinimum")]
    public double? DayMinimum { get; set; }

    /// <summary>
    /// Moment (hour) of day from this forecast
    /// </summary>
    [JsonPropertyName("dayMaximum")]
    public double? DayMaximum { get; set; }

    // Propiedades de compatibilidad para mantener funcionalidad existente
    
    /// <summary>
    /// Humedad en porcentaje (calculada desde RelativeHumidity para compatibilidad)
    /// </summary>
    [JsonIgnore]
    public double? Humidity => RelativeHumidity;

    /// <summary>
    /// Timestamp para compatibilidad con código existente
    /// </summary>
    [JsonIgnore]
    public DateTime Timestamp => DateRetrieved ?? DateTime.UtcNow;

    /// <summary>
    /// Unidad de temperatura (siempre Celsius según FIWARE)
    /// </summary>
    [JsonIgnore]
    public string Unit => "Celsius";

    /// <summary>
    /// Descripción textual de la ubicación basada en coordenadas o dirección
    /// </summary>
    [JsonIgnore]
    public string LocationDescription
    {
        get
        {
            if (Address != null)
            {
                var parts = new List<string>();
                if (!string.IsNullOrEmpty(Address.StreetAddress)) parts.Add(Address.StreetAddress);
                if (!string.IsNullOrEmpty(Address.AddressLocality)) parts.Add(Address.AddressLocality);
                if (!string.IsNullOrEmpty(Address.AddressCountry)) parts.Add(Address.AddressCountry);
                if (parts.Any()) return string.Join(", ", parts);
            }

            if (Location?.Type == "Point" && Location.Coordinates?.Length >= 2)
            {
                return $"Lat: {Location.Coordinates[1]:F4}, Lon: {Location.Coordinates[0]:F4}";
            }

            return AreaServed ?? "Ubicación desconocida";
        }
    }

    /// <summary>
    /// Fecha y hora para compatibilidad con código existente
    /// </summary>
    [JsonIgnore]
    public DateTime DateTime => ValidFrom ?? DateRetrieved ?? DateTime.UtcNow;
}

/// <summary>
/// GeoJSON geometry according to FIWARE standard
/// </summary>
public class GeoJsonGeometry
{
    /// <summary>
    /// GeoJSON type (Point, LineString, Polygon, etc.)
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = "Point";

    /// <summary>
    /// GeoJSON coordinates array [longitude, latitude] for Point type
    /// </summary>
    [JsonPropertyName("coordinates")]
    public double[] Coordinates { get; set; } = new double[2];
}

/// <summary>
/// Address according to FIWARE civic address standard
/// </summary>
public class Address
{
    /// <summary>
    /// The street address
    /// </summary>
    [JsonPropertyName("streetAddress")]
    public string? StreetAddress { get; set; }

    /// <summary>
    /// The locality in which the street address is, and which is in the region
    /// </summary>
    [JsonPropertyName("addressLocality")]
    public string? AddressLocality { get; set; }

    /// <summary>
    /// The region in which the locality is, and which is in the country
    /// </summary>
    [JsonPropertyName("addressRegion")]
    public string? AddressRegion { get; set; }

    /// <summary>
    /// The postal code
    /// </summary>
    [JsonPropertyName("postalCode")]
    public string? PostalCode { get; set; }

    /// <summary>
    /// The country name
    /// </summary>
    [JsonPropertyName("addressCountry")]
    public string? AddressCountry { get; set; }

    /// <summary>
    /// The post office box number
    /// </summary>
    [JsonPropertyName("postOfficeBoxNumber")]
    public string? PostOfficeBoxNumber { get; set; }
}

// Alias para compatibilidad con código existente
public class Location : GeoJsonGeometry
{
    // Mantener compatibilidad con código existente
}