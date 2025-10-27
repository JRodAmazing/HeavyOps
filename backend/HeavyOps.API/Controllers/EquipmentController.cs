using Microsoft.AspNetCore.Mvc;

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
    public IActionResult Get()
    {
        return Ok(_equipment);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(string id)
    {
        var eq = _equipment.FirstOrDefault(e => e.Id == id);
        return eq == null ? NotFound() : Ok(eq);
    }
}

public class Equipment
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public int Hours { get; set; }
}