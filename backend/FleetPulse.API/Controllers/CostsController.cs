using Microsoft.AspNetCore.Mvc;
using FleetPulse.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FleetPulse.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CostsController : ControllerBase
    {
        private static List<CostEntry> costs = new()
        {
            new CostEntry
            {
                Id = "cost_001",
                ProjectId = "proj_roadwork_2025",
                Category = "equipment",
                Description = "CAT320 Excavator - 240 hours @ /day",
                Amount = 85000,
                EquipmentId = "CAT320",
                DateIncurred = new DateTime(2025, 10, 15)
            },
            new CostEntry
            {
                Id = "cost_002",
                ProjectId = "proj_roadwork_2025",
                Category = "fuel",
                Description = "Diesel fuel for equipment",
                Amount = 12500,
                DateIncurred = new DateTime(2025, 10, 20)
            },
            new CostEntry
            {
                Id = "cost_003",
                ProjectId = "proj_roadwork_2025",
                Category = "labor",
                Description = "Equipment operators - 480 hours",
                Amount = 28000,
                DateIncurred = new DateTime(2025, 10, 31)
            }
        };

        [HttpGet]
        public ActionResult<IEnumerable<CostEntry>> GetAllCosts()
        {
            return Ok(costs);
        }

        [HttpGet("project/{projectId}")]
        public ActionResult<object> GetCostsByProject(string projectId)
        {
            var projectCosts = costs.Where(c => c.ProjectId == projectId).ToList();
            var totalCost = projectCosts.Sum(c => c.Amount);

            return Ok(new
            {
                projectId,
                entries = projectCosts,
                total = totalCost,
                breakdown = projectCosts
                    .GroupBy(c => c.Category)
                    .Select(g => new { category = g.Key, amount = g.Sum(c => c.Amount) })
                    .ToList()
            });
        }

        [HttpGet("{id}")]
        public ActionResult<CostEntry> GetCostById(string id)
        {
            var cost = costs.FirstOrDefault(c => c.Id == id);
            if (cost == null)
                return NotFound(new { message = $"Cost entry {id} not found" });
            return Ok(cost);
        }

        [HttpPost]
        public ActionResult<CostEntry> CreateCost([FromBody] CostEntry newCost)
        {
            if (string.IsNullOrEmpty(newCost.ProjectId))
                return BadRequest(new { message = "ProjectId is required" });

            newCost.Id = Guid.NewGuid().ToString();
            newCost.CreatedAt = DateTime.UtcNow;
            costs.Add(newCost);

            return CreatedAtAction(nameof(GetCostById), new { id = newCost.Id }, newCost);
        }

        [HttpPut("{id}")]
        public ActionResult<CostEntry> UpdateCost(string id, [FromBody] CostEntry updatedCost)
        {
            var cost = costs.FirstOrDefault(c => c.Id == id);
            if (cost == null)
                return NotFound(new { message = $"Cost entry {id} not found" });

            cost.Description = updatedCost.Description ?? cost.Description;
            cost.Amount = updatedCost.Amount > 0 ? updatedCost.Amount : cost.Amount;

            return Ok(cost);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteCost(string id)
        {
            var cost = costs.FirstOrDefault(c => c.Id == id);
            if (cost == null)
                return NotFound(new { message = $"Cost entry {id} not found" });

            costs.Remove(cost);
            return Ok(new { message = $"Cost entry {id} deleted" });
        }
    }
}
