using Microsoft.AspNetCore.Mvc;

namespace FleetPulse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipmentController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var equipment = new[]
        {
            new { id = "CAT320", name = "Caterpillar 320D", status = "operational", hours = 2450 },
            new { id = "KOMATSU350", name = "Komatsu PC350", status = "operational", hours = 3120 },
            new { id = "VOLVO240", name = "Volvo EC240B", status = "maintenance", hours = 1890 }
        };
        return Ok(equipment);
    }
}
