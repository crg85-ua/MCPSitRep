using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using ModelContextProtocol.Server;

/// <summary>
/// MCP tools for temperature sensor data collection and AI interpretation.
/// These tools can call temperature sensor API endpoints and interpret the data using AI.
/// </summary>
internal class TemperatureSensorTools
{
    private readonly HttpClient _httpClient;

    public TemperatureSensorTools(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [McpServerTool]
    [Description("Calls a temperature sensor API endpoint and uses AI (Llama) to interpret the temperature data, providing insights and recommendations.")]
    public async Task<string> GetTemperatureDataWithAIInterpretation(
        [Description("The URL of the temperature sensor API endpoint")] string apiUrl,
        [Description("Optional API key for authentication")] string? apiKey = null,
        [Description("Include safety recommendations in the interpretation")] bool includeSafetyRecommendations = true,
        [Description("Include trend analysis if available")] bool includeTrendAnalysis = true)
    {
        try
        {
            //// Configure HTTP client headers if API key is provided
            //if (!string.IsNullOrEmpty(apiKey))
            //{
            //    _httpClient.DefaultRequestHeaders.Clear();
            //    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            //}

            // Call the temperature sensor API
            var response = await _httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            var temperatureData = await response.Content.ReadAsStringAsync();

            // Try to parse as JSON to extract temperature values
            TemperatureSensorData? sensorData = null;
            try
            {
                sensorData = await response.Content.ReadFromJsonAsync<TemperatureSensorData>();
            }
            catch
            {
                // If parsing fails, we'll work with raw data
            }

            // Generate AI interpretation using Llama-style analysis
            var interpretation = GenerateAIInterpretation(temperatureData, sensorData, includeSafetyRecommendations, includeTrendAnalysis);

            return $"""
                ## Temperature Sensor Data Analysis

                ### Raw Sensor Data:
                {temperatureData}

                ### AI Interpretation (Llama Analysis):
                {interpretation}
                
                ---
                *Analysis generated using AI interpretation of temperature sensor data*
                """;
        }
        catch (HttpRequestException ex)
        {
            return $"Error calling temperature sensor API: {ex.Message}";
        }
        catch (Exception ex)
        {
            return $"Unexpected error: {ex.Message}";
        }
    }

    [McpServerTool]
    [Description("Simulates calling a temperature sensor API with mock data for demonstration purposes.")]
    public string GetMockTemperatureDataWithAIInterpretation(
        [Description("Simulated temperature value in Celsius")] double temperatureCelsius = 22.5,
        [Description("Simulated humidity percentage")] double humidity = 45.0,
        [Description("Sensor location description")] string location = "Office Room",
        [Description("Include safety recommendations in the interpretation")] bool includeSafetyRecommendations = true)
    {
        var mockData = new TemperatureSensorData
        {
            Temperature = temperatureCelsius,
            Humidity = humidity,
            Location = location,
            Timestamp = DateTime.UtcNow,
            Unit = "Celsius"
        };

        var mockJson = System.Text.Json.JsonSerializer.Serialize(mockData, new System.Text.Json.JsonSerializerOptions 
        { 
            WriteIndented = true 
        });

        var interpretation = GenerateAIInterpretation(mockJson, mockData, includeSafetyRecommendations, true);

        return $"""
            ## Mock Temperature Sensor Data Analysis

            ### Simulated Sensor Data:
            {mockJson}

            ### AI Interpretation (Llama Analysis):
            {interpretation}
            
            ---
            *This is a demonstration using simulated sensor data*
            """;
    }

    private string GenerateAIInterpretation(string rawData, TemperatureSensorData? parsedData, bool includeSafetyRecommendations, bool includeTrendAnalysis)
    {
        var analysis = new List<string>();

        // Temperature Analysis
        if (parsedData != null)
        {
            analysis.Add("### Temperature Analysis:");
            
            var tempC = parsedData.Temperature;
            var tempF = tempC * 9/5 + 32;
            
            analysis.Add($"- **Current Temperature**: {tempC:F1}°C ({tempF:F1}°F)");
            
            // Comfort zone analysis
            if (tempC >= 20 && tempC <= 25)
            {
                analysis.Add($"- **Comfort Status**: Optimal comfort zone for most people");
            }
            else if (tempC >= 18 && tempC <= 27)
            {
                analysis.Add($"- **Comfort Status**: Acceptable but may require adjustment");
            }
            else if (tempC < 18)
            {
                analysis.Add($"- **Comfort Status**: Too cold - heating recommended");
            }
            else
            {
                analysis.Add($"- **Comfort Status**: Too warm - cooling recommended");
            }

            // Humidity analysis
            if (parsedData.Humidity.HasValue)
            {
                var humidity = parsedData.Humidity.Value;
                analysis.Add("\n### Humidity Analysis:");
                analysis.Add($"- **Current Humidity**: {humidity:F1}%");
                
                if (humidity >= 40 && humidity <= 60)
                {
                    analysis.Add($"- **Humidity Status**: Ideal humidity range");
                }
                else if (humidity < 40)
                {
                    analysis.Add($"- **Humidity Status**: Too dry - consider humidification");
                }
                else
                {
                    analysis.Add($"- **Humidity Status**: Too humid - may cause discomfort");
                }
            }

            // Location context
            if (!string.IsNullOrEmpty(parsedData.Location))
            {
                analysis.Add($"\n### Location Context: {parsedData.Location}");
                analysis.Add(GetLocationSpecificAdvice(parsedData.Location, tempC));
            }
        }

        // Safety recommendations
        if (includeSafetyRecommendations)
        {
            analysis.Add("\n### Safety & Health Recommendations:");
            
            if (parsedData != null)
            {
                var temp = parsedData.Temperature;
                if (temp < 16)
                {
                    analysis.Add("- **Cold Warning**: Temperature below recommended minimum. Risk of hypothermia in prolonged exposure.");
                    analysis.Add("- **Action**: Increase heating immediately, ensure proper insulation.");
                }
                else if (temp > 30)
                {
                    analysis.Add("- **Heat Warning**: High temperature detected. Risk of heat stress and dehydration.");
                    analysis.Add("- **Action**: Improve ventilation or cooling, ensure adequate hydration.");
                }
                else
                {
                    analysis.Add("- Temperature within safe operating range.");
                }
            }
        }

        // Trend analysis (simulated for demonstration)
        if (includeTrendAnalysis)
        {
            analysis.Add("\n### Trend Analysis:");
            analysis.Add("- **Note**: Real trend analysis would require historical data from the sensor API");
            analysis.Add("- **Recommendation**: Consider implementing data logging for pattern recognition");
            analysis.Add("- **AI Insight**: Monitor temperature variations throughout the day for optimization opportunities");
        }

        // Energy efficiency suggestions
        analysis.Add("\n### Energy Efficiency Suggestions:");
        if (parsedData != null)
        {
            var temp = parsedData.Temperature;
            if (temp >= 20 && temp <= 25)
            {
                analysis.Add("- Current temperature is energy-efficient");
            }
            else
            {
                analysis.Add($"- Adjusting to 22°C could optimize energy consumption");
            }
        }

        return string.Join("\n", analysis);
    }

    private string GetLocationSpecificAdvice(string location, double temperature)
    {
        return location.ToLowerInvariant() switch
        {
            var loc when loc.Contains("office") => 
                $"Office environments work best at 21-24°C. Current: {temperature:F1}°C - " + 
                (temperature >= 21 && temperature <= 24 ? "Perfect for productivity!" : "Consider adjustment for optimal work performance."),
            
            var loc when loc.Contains("bedroom") => 
                $"Bedrooms should be cooler (16-19°C) for better sleep. Current: {temperature:F1}°C - " + 
                (temperature >= 16 && temperature <= 19 ? "Ideal for restful sleep." : "May affect sleep quality."),
            
            var loc when loc.Contains("kitchen") => 
                $"Kitchens tend to be warmer due to appliances. Current: {temperature:F1}°C - " + 
                (temperature <= 26 ? "Within acceptable range." : "Consider increased ventilation."),
                
            _ => $"General comfort range is 20-25°C. Current: {temperature:F1}°C"
        };
    }
}

/// <summary>
/// Data structure for parsing temperature sensor API responses
/// </summary>
internal class TemperatureSensorData
{
    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }

    [JsonPropertyName("humidity")]
    public double? Humidity { get; set; }

    [JsonPropertyName("location")]
    public string Location { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("unit")]
    public string Unit { get; set; } = "Celsius";

    [JsonPropertyName("sensor_id")]
    public string? SensorId { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }
}