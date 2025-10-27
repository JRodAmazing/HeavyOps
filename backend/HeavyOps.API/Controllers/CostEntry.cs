namespace HeavyOps.API.Models;

public class CostEntry
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ProjectId { get; set; } = "";
    public string Category { get; set; } = ""; // labor, parts, equipment, fuel, other
    public string Description { get; set; } = "";
    public decimal Amount { get; set; }
    public DateTime DateIncurred { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "pending"; // pending, approved, paid
    public string Notes { get; set; } = "";
    public string CreatedBy { get; set; } = "";
}