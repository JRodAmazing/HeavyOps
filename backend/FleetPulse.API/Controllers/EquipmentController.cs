using Microsoft.AspNetCore.Mvc;
using FleetPulse.API.Models;

namespace FleetPulse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipmentController : ControllerBase
{
    private static List<Equipment> _equipment = new()
    {
        new Equipment
        {
            Id = "CAT320",
            Name = "Caterpillar 320D",
            Status = "operational",
            Hours = 2450,
            ServiceLogs = new()
            {
                new ServiceLog
                {
                    DatePerformed = DateTime.UtcNow.AddDays(-7),
                    HoursAtService = 2400,
                    Notes = "Oil change and filter replacement",
                    LaborCost = 150
                }
            }
        },
        new Equipment
        {
            Id = "KOMATSU350",
            Name = "Komatsu PC350",
            Status = "operational",
            Hours = 3120
        },
        new Equipment
        {
            Id = "VOLVO240",
            Name = "Volvo EC240B",
            Status = "operational",
            Hours = 1850
        }
    };

    // GET all equipment
    [HttpGet]
    public ActionResult<List<Equipment>> GetAll() => Ok(_equipment);

    // GET equipment by ID
    [HttpGet("{id}")]
    public ActionResult<Equipment> GetById(string id)
    {
        var equipment = _equipment.FirstOrDefault(e => e.Id == id);
        return equipment == null ? NotFound() : Ok(equipment);
    }

    // GET logs for equipment
    [HttpGet("{id}/logs")]
    public ActionResult<List<ServiceLog>> GetLogs(string id)
    {
        var equipment = _equipment.FirstOrDefault(e => e.Id == id);
        return equipment == null ? NotFound() : Ok(equipment.ServiceLogs);
    }

    // POST new log
    [HttpPost("{id}/logs")]
    public ActionResult<ServiceLog> AddLog(string id, [FromBody] ServiceLog log)
    {
        var equipment = _equipment.FirstOrDefault(e => e.Id == id);
        if (equipment == null) return NotFound();

        log.EquipmentId = id;
        log.Id = Guid.NewGuid().ToString();
        equipment.ServiceLogs.Add(log);

        return CreatedAtAction(nameof(GetLogs), new { id }, log);
    }
}