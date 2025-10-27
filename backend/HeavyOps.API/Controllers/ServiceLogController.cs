using Microsoft.AspNetCore.Mvc;
using HeavyOps.API.Models;

namespace HeavyOps.API.Controllers;

[ApiController]
[Route("api/service-logs")]
public class ServiceLogController : ControllerBase
{
    private static List<ServiceLog> _serviceLogs = new();

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_serviceLogs);
    }

    [HttpGet("project/{projectId}")]
    public IActionResult GetByProject(string projectId)
    {
        var logs = _serviceLogs.Where(l => l.ProjectId == projectId).OrderByDescending(l => l.DatePerformed).ToList();
        return Ok(logs);
    }

    [HttpGet("equipment/{equipmentId}")]
    public IActionResult GetByEquipment(string equipmentId)
    {
        var logs = _serviceLogs.Where(l => l.EquipmentId == equipmentId).OrderByDescending(l => l.DatePerformed).ToList();
        return Ok(logs);
    }

    [HttpGet("project/{projectId}/equipment/{equipmentId}")]
    public IActionResult GetByProjectAndEquipment(string projectId, string equipmentId)
    {
        var logs = _serviceLogs
            .Where(l => l.ProjectId == projectId && l.EquipmentId == equipmentId)
            .OrderByDescending(l => l.DatePerformed)
            .ToList();
        return Ok(logs);
    }

    [HttpGet("project/{projectId}/summary")]
    public IActionResult GetProjectServiceSummary(string projectId)
    {
        var logs = _serviceLogs.Where(l => l.ProjectId == projectId).ToList();
        var summary = new
        {
            totalLogs = logs.Count,
            totalHoursWorked = logs.Sum(l => l.HoursWorked),
            totalLaborCost = logs.Sum(l => l.LaborCost),
            totalPartsUsed = logs.Sum(l => l.PartsUsed),
            byServiceType = logs
                .GroupBy(l => l.ServiceType)
                .Select(g => new { type = g.Key, count = g.Count(), hours = g.Sum(l => l.HoursWorked), cost = g.Sum(l => l.LaborCost) })
                .ToList(),
            recentLogs = logs.Take(5).ToList()
        };
        return Ok(summary);
    }

    [HttpPost]
    public IActionResult Create([FromBody] ServiceLog log)
    {
        if (log == null || string.IsNullOrEmpty(log.ProjectId) || string.IsNullOrEmpty(log.EquipmentId))
            return BadRequest("ProjectId and EquipmentId are required");

        _serviceLogs.Add(log);
        return CreatedAtAction(nameof(GetByProject), new { projectId = log.ProjectId }, log);
    }

    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] ServiceLog log)
    {
        var existing = _serviceLogs.FirstOrDefault(l => l.Id == id);
        if (existing == null)
            return NotFound();

        existing.ServiceType = log.ServiceType;
        existing.Description = log.Description;
        existing.HoursWorked = log.HoursWorked;
        existing.LaborCost = log.LaborCost;
        existing.PartsUsed = log.PartsUsed;
        existing.Technician = log.Technician;
        existing.Status = log.Status;
        existing.Notes = log.Notes;
        existing.DatePerformed = log.DatePerformed;

        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        var log = _serviceLogs.FirstOrDefault(l => l.Id == id);
        if (log == null)
            return NotFound();

        _serviceLogs.Remove(log);
        return NoContent();
    }
}