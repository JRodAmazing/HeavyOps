using System;
using System.Collections.Generic;

namespace FleetPulse.API.Models
{
    public class Project
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "";
        public string Location { get; set; } = "";
        public string Description { get; set; } = "";
        public string ProjectManager { get; set; } = "";
        
        public decimal Budget { get; set; }
        public decimal TotalSpent { get; set; }
        
        public string Status { get; set; } = "planning";
        
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public List<EquipmentAssignment> Assignments { get; set; } = new();
        public List<CostEntry> Costs { get; set; } = new();
    }
}
