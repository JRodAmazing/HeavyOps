using Microsoft.AspNetCore.Mvc;
using FleetPulse.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FleetPulse.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssignmentsController : ControllerBase
    {
        private static List<EquipmentAssignment> assignments = new()
        {
            new EquipmentAssignment
            {
                Id = "assign_001",
                ProjectId = "proj_roadwork_2025",
                EquipmentId = "CAT320",
                Role = "primary_excavator",
                DailyRate = 850,
                HoursUsed = 240,
                Status = "active",
                AssignedDate = new DateTime(2025, 10, 1)
            },
            new EquipmentAssignment
            {
                Id = "assign_002",
                ProjectId = "proj_roadwork_2025",
                EquipmentId = "KOMATSU350",
                Role = "support",
                DailyRate = 750,
                HoursUsed = 180,
                Status = "active",
                AssignedDate = new DateTime(2025, 10, 5)
            }
        };

        [HttpGet]
        public ActionResult<IEnumerable<EquipmentAssignment>> GetAllAssignments()
        {
            return Ok(assignments);
        }

        [HttpGet("project/{projectId}")]
        public ActionResult<IEnumerable<EquipmentAssignment>> GetAssignmentsByProject(string projectId)
        {
            var projectAssignments = assignments.Where(a => a.ProjectId == projectId).ToList();
            return Ok(projectAssignments);
        }

        [HttpGet("equipment/{equipmentId}")]
        public ActionResult<IEnumerable<EquipmentAssignment>> GetAssignmentsByEquipment(string equipmentId)
        {
            var equipmentAssignments = assignments.Where(a => a.EquipmentId == equipmentId).ToList();
            return Ok(equipmentAssignments);
        }

        [HttpGet("{id}")]
        public ActionResult<EquipmentAssignment> GetAssignmentById(string id)
        {
            var assignment = assignments.FirstOrDefault(a => a.Id == id);
            if (assignment == null)
                return NotFound(new { message = $"Assignment {id} not found" });
            return Ok(assignment);
        }

        [HttpPost]
        public ActionResult<EquipmentAssignment> CreateAssignment([FromBody] EquipmentAssignment newAssignment)
        {
            if (string.IsNullOrEmpty(newAssignment.ProjectId))
                return BadRequest(new { message = "ProjectId is required" });

            newAssignment.Id = Guid.NewGuid().ToString();
            newAssignment.CreatedAt = DateTime.UtcNow;
            assignments.Add(newAssignment);

            return CreatedAtAction(nameof(GetAssignmentById), new { id = newAssignment.Id }, newAssignment);
        }

        [HttpPut("{id}")]
        public ActionResult<EquipmentAssignment> UpdateAssignment(string id, [FromBody] EquipmentAssignment updatedAssignment)
        {
            var assignment = assignments.FirstOrDefault(a => a.Id == id);
            if (assignment == null)
                return NotFound(new { message = $"Assignment {id} not found" });

            assignment.HoursUsed = updatedAssignment.HoursUsed >= 0 ? updatedAssignment.HoursUsed : assignment.HoursUsed;
            assignment.Status = updatedAssignment.Status ?? assignment.Status;

            return Ok(assignment);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteAssignment(string id)
        {
            var assignment = assignments.FirstOrDefault(a => a.Id == id);
            if (assignment == null)
                return NotFound(new { message = $"Assignment {id} not found" });

            assignments.Remove(assignment);
            return Ok(new { message = $"Assignment {id} deleted" });
        }
    }
}
