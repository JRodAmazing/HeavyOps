namespace HeavyOps.Data.Models;

public class EquipmentAssignment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ProjectId { get; set; } = "";
    public string EquipmentId { get; set; } = "";
    public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
    public DateTime? UnassignedDate { get; set; }
    public string Status { get; set; } = "active";
    
    // Additional properties being referenced
    public decimal DailyRate { get; set; }
    public int EstimatedDays { get; set; }
}
