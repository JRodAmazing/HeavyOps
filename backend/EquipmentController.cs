using Microsoft.AspNetCore.Mvc;

namespace HeavyOps.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipmentController : ControllerBase
{
    private static List<Equipment> _equipment = new()
    {
        new Equipment { Id = "CAT320", Name = "Caterpillar 320D", Status = "operational", Hours = 2450 },
        new Equipment { Id = "KOMATSU350", Name = "Komatsu PC350", Status = "operational", Hours = 3120 },
        new Equipment { Id = "VOLVO240", Name = "Volvo EC240B", Status = "maintenance", Hours = 1890 }
    };

    [HttpGet]
    public ActionResult<List<Equipment>> GetAll()
    {
        return Ok(_equipment);
    }

    [HttpGet("{id}")]
    public ActionResult<Equipment> GetById(string id)
    {
        var equipment = _equipment.FirstOrDefault(e => e.Id == id);
        if (equipment == null)
            return NotFound();
        return Ok(equipment);
    }

    [HttpPost]
    public ActionResult<Equipment> Create([FromBody] Equipment equipment)
    {
        equipment.Id = Guid.NewGuid().ToString();
        _equipment.Add(equipment);
        return CreatedAtAction(nameof(GetById), new { id = equipment.Id }, equipment);
    }

    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] Equipment equipment)
    {
        var existing = _equipment.FirstOrDefault(e => e.Id == id);
        if (existing == null)
            return NotFound();

        existing.Name = equipment.Name;
        existing.Status = equipment.Status;
        existing.Hours = equipment.Hours;

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        var equipment = _equipment.FirstOrDefault(e => e.Id == id);
        if (equipment == null)
            return NotFound();

        _equipment.Remove(equipment);
        return NoContent();
    }
}

public class Equipment
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Status { get; set; } = "operational";
    public int Hours { get; set; }
}