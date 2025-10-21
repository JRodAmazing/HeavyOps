namespace FleetPulse.API.Models;

public class Equipment
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Status { get; set; } = "operational";
    public int Hours { get; set; }
    public List<ServiceLog> ServiceLogs { get; set; } = new();
}
