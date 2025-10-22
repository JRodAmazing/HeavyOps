namespace FleetPulse.API.Models
{
    public class DiagnosticFrame
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string EquipmentId { get; set; } = "";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string CanId { get; set; } = "";
        public string Data { get; set; } = "";
    }
}
