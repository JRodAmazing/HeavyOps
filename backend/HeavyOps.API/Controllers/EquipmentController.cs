using Microsoft.AspNetCore.Mvc;
using HeavyOps.Data.Models;

namespace HeavyOps.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipmentController : ControllerBase
{
    private static readonly List<Equipment> Equipment = new()
    {
        new Equipment { Id = "CAT320", Name = "Caterpillar 320D", Status = "operational", Hours = 2450 },
        new Equipment { Id = "KOMATSU350", Name = "Komatsu PC350", Status = "operational", Hours = 3120 },
        new Equipment { Id = "VOLVO240", Name = "Volvo EC240B", Status = "operational", Hours = 1890 }
    };

    [HttpGet]
    public ActionResult<List<Equipment>> GetAllEquipment()
    {
        return Ok(Equipment);
    }

    [HttpGet("{id}")]
    public ActionResult<Equipment> GetEquipment(string id)
    {
        var equipment = Equipment.FirstOrDefault(e => e.Id == id);
        if (equipment == null)
            return NotFound();
        return Ok(equipment);
    }

    [HttpPost]
    public ActionResult<Equipment> CreateEquipment([FromBody] Equipment equipment)
    {
        equipment.Id = Guid.NewGuid().ToString();
        Equipment.Add(equipment);
        return CreatedAtAction(nameof(GetEquipment), new { id = equipment.Id }, equipment);
    }
}
