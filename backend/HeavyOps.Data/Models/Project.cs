namespace HeavyOps.Data.Models;

public class Project
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public string Location { get; set; } = "";
    public decimal Budget { get; set; }
    public string ProjectManager { get; set; } = "";
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = "active";
    public List<string> EquipmentIds { get; set; } = new();
    public List<CostEntry> Costs { get; set; } = new();
}
