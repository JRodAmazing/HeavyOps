using Microsoft.AspNetCore.Mvc;
using HeavyOps.API.Models;

namespace HeavyOps.API.Controllers;

[ApiController]
[Route("api/costs")]
public class CostEntryController : ControllerBase
{
    private static List<CostEntry> _costs = new();

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_costs);
    }

    [HttpGet("project/{projectId}")]
    public IActionResult GetByProject(string projectId)
    {
        var costs = _costs.Where(c => c.ProjectId == projectId).OrderByDescending(c => c.DateIncurred).ToList();
        return Ok(costs);
    }

    [HttpGet("project/{projectId}/summary")]
    public IActionResult GetProjectCostSummary(string projectId)
    {
        var costs = _costs.Where(c => c.ProjectId == projectId).ToList();
        var summary = new
        {
            totalCosts = costs.Sum(c => c.Amount),
            byCategoryBreakdown = costs
                .GroupBy(c => c.Category)
                .Select(g => new { category = g.Key, total = g.Sum(c => c.Amount), count = g.Count() })
                .ToList(),
            byStatusBreakdown = costs
                .GroupBy(c => c.Status)
                .Select(g => new { status = g.Key, total = g.Sum(c => c.Amount), count = g.Count() })
                .ToList(),
            approvedCosts = costs.Where(c => c.Status == "approved").Sum(c => c.Amount),
            pendingCosts = costs.Where(c => c.Status == "pending").Sum(c => c.Amount),
        };
        return Ok(summary);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CostEntry cost)
    {
        if (cost == null || string.IsNullOrEmpty(cost.ProjectId))
            return BadRequest("ProjectId is required");

        _costs.Add(cost);
        return CreatedAtAction(nameof(GetByProject), new { projectId = cost.ProjectId }, cost);
    }

    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] CostEntry cost)
    {
        var existing = _costs.FirstOrDefault(c => c.Id == id);
        if (existing == null)
            return NotFound();

        existing.Category = cost.Category;
        existing.Description = cost.Description;
        existing.Amount = cost.Amount;
        existing.Status = cost.Status;
        existing.Notes = cost.Notes;
        existing.DateIncurred = cost.DateIncurred;

        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        var cost = _costs.FirstOrDefault(c => c.Id == id);
        if (cost == null)
            return NotFound();

        _costs.Remove(cost);
        return NoContent();
    }

    [HttpPatch("{id}/approve")]
    public IActionResult Approve(string id)
    {
        var cost = _costs.FirstOrDefault(c => c.Id == id);
        if (cost == null)
            return NotFound();

        cost.Status = "approved";
        return Ok(cost);
    }
}