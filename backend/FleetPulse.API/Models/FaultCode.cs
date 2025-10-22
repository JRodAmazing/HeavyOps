namespace FleetPulse.API.Models;

public class FaultCode
{
    public string Code { get; set; } = "";              // "P123F"
    public string Description { get; set; } = "";       // "Engine Oil Pressure Low"
    public string[] PossibleCauses { get; set; } = Array.Empty<string>();
    public string[] SuggestedFixes { get; set; } = Array.Empty<string>();
    public string Severity { get; set; } = "info";      // "critical", "warning", "info"
    public string Component { get; set; } = "";         // "engine", "transmission", "hydraulic"
}
