namespace HeavyOps.Data.Models;

public class CostEntry
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ProjectId { get; set; } = "";
    public string Category { get; set; } = "";
    public decimal Amount { get; set; }
    public string Description { get; set; } = "";
    public DateTime Date { get; set; } = DateTime.UtcNow;
}
