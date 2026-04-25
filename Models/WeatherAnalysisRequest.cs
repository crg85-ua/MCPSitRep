public class WeatherAnalysisRequest
{
    public required string IdEspacio { get; set; }
    public bool IncludeSafetyRecommendations { get; set; } = true;
    public bool IncludeTrendAnalysis { get; set; } = true;
}
