using System;

namespace FleetPulse.API.Models
{
    public class DiagnosticFrame
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string EquipmentId { get; set; } = "";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string CanId { get; set; } = "";  // e.g., "0x0CF00400"
        public string Data { get; set; } = "";   // hex string, e.g., "1122334455667788"
    }
}