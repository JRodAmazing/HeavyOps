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
            Hours = 3120,
            ServiceLogs = new()
            {
                new ServiceLog
                {
                    DatePerformed = DateTime.UtcNow.AddDays(-14),
                    HoursAtService = 3100,
                    Notes = "Bucket cylinder replacement and hydraulic fluid top-up",
                    LaborCost = 425
                },
                new ServiceLog
                {
                    DatePerformed = DateTime.UtcNow.AddDays(-3),
                    HoursAtService = 3115,
                    Notes = "Engine belt inspection",
                    LaborCost = 75
                }
            }
        },
        new Equipment
        {
            Id = "VOLVO240",
            Name = "Volvo EC240B",
            Status = "operational",
            Hours = 1850,
            ServiceLogs = new()
            {
                new ServiceLog
                {
                    DatePerformed = DateTime.UtcNow.AddDays(-21),
                    HoursAtService = 1800,
                    Notes = "Major service: oil change, filter replacement, coolant flush",
                    LaborCost = 350
                }
            }
        }
    };

    // Static method for Reports controller to access equipment
    public static List<Equipment> GetEquipmentList() => _equipment;

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
