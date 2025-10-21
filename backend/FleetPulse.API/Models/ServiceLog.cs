using System;

namespace FleetPulse.API.Models;

public class ServiceLog
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EquipmentId { get; set; } = "";
    public DateTime DatePerformed { get; set; } = DateTime.UtcNow;
    public int HoursAtService { get; set; }
    public string Notes { get; set; } = "";
    public decimal LaborCost { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
