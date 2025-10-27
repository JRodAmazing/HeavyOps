namespace HeavyOps.Data.Models;

public class ServiceLog
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EquipmentId { get; set; } = "";
    public string ProjectId { get; set; } = "";
    public DateTime DatePerformed { get; set; } = DateTime.UtcNow;
    public int HoursAtService { get; set; }
    public string Notes { get; set; } = "";
    public decimal LaborCost { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Additional properties being referenced in controllers
    public string ServiceType { get; set; } = "";
    public string Description { get; set; } = "";
    public int HoursWorked { get; set; }
    public string PartsUsed { get; set; } = "";
    public string Technician { get; set; } = "";
    public string Status { get; set; } = "completed";
}
