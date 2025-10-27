using System;

namespace FleetPulse.API.Models
{
    public class EquipmentAssignment
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ProjectId { get; set; } = "";
        public string EquipmentId { get; set; } = "";
        
        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ReturnDate { get; set; }
        public string Role { get; set; } = "";
        public decimal DailyRate { get; set; }
        
        public int HoursUsed { get; set; }
        public string Status { get; set; } = "active";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
