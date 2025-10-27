using System;

namespace FleetPulse.API.Models
{
    public class CostEntry
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ProjectId { get; set; } = "";
        
        public string Category { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Amount { get; set; }
        
        public string EquipmentId { get; set; } = "";
        public string Notes { get; set; } = "";
        
        public DateTime DateIncurred { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
