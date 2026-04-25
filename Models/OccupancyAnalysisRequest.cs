public class OccupancyAnalysisRequest
{
    public required string IdEspacio { get; set; }
    public bool IncludeAlerts { get; set; } = true;
}
