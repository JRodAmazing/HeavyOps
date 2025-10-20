using Microsoft.AspNetCore.Mvc;

namespace FleetPulse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipmentController : ControllerBase
{
    private static readonly List<Equipment> Equipment = new()
    {
        new Equipment { Id = "CAT320", Name = "Caterpillar 320D", Status = "operational", Hours = 2450 },
        new Equipment { Id = "KOMATSU350", Name = "Komatsu PC350", Status = "operational", Hours = 3120 },
        new Equipment { Id = "VOLVO240", Name = "Volvo EC240B", Status = "maintenance", Hours = 1890 }
    };

    [HttpGet]
    public ActionResult<IEnumerable<EquipmentDto>> Get()
    {
        return Ok(Equipment);
    }

    [HttpGet("{id}")]
    public ActionResult<EquipmentDto> GetById(string id)
    {
        var item = Equipment.FirstOrDefault(e => e.Id == id);
        return item == null ? NotFound() : Ok(item);
    }
}

public class EquipmentDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public int Hours { get; set; }
}

public class Equipment
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public int Hours { get; set; }
}
