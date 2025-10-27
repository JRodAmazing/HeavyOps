using Microsoft.AspNetCore.Mvc;
using HeavyOps.API.Models;

namespace HeavyOps.API.Controllers;

[ApiController]
[Route("api/equipment-assignments")]
public class EquipmentAssignmentController : ControllerBase
{
    private static List<EquipmentAssignment> _assignments = new();

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_assignments);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(string id)
    {
        var assignment = _assignments.FirstOrDefault(a => a.Id == id);
        if (assignment == null)
            return NotFound();
        return Ok(assignment);
    }

    [HttpGet("project/{projectId}")]
    public IActionResult GetByProject(string projectId)
    {
        var assignments = _assignments.Where(a => a.ProjectId == projectId).ToList();
        return Ok(assignments);
    }

    [HttpPost]
    public IActionResult Create([FromBody] EquipmentAssignment assignment)
    {
        if (assignment == null || string.IsNullOrEmpty(assignment.ProjectId) || string.IsNullOrEmpty(assignment.EquipmentId))
            return BadRequest("ProjectId and EquipmentId are required");

        _assignments.Add(assignment);
        return CreatedAtAction(nameof(GetById), new { id = assignment.Id }, assignment);
    }

    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] EquipmentAssignment assignment)
    {
        var existing = _assignments.FirstOrDefault(a => a.Id == id);
        if (existing == null)
            return NotFound();

        existing.Status = assignment.Status;
        existing.DailyRate = assignment.DailyRate;
        existing.EstimatedDays = assignment.EstimatedDays;
        existing.Notes = assignment.Notes;
        existing.UnassignedDate = assignment.UnassignedDate;

        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        var assignment = _assignments.FirstOrDefault(a => a.Id == id);
        if (assignment == null)
            return NotFound();

        _assignments.Remove(assignment);
        return NoContent();
    }
}