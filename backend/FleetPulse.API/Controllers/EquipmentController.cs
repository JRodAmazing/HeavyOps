using Microsoft.AspNetCore.Mvc;
using FleetPulse.API.Models;

namespace FleetPulse.API.Controllers
{
    [ApiController]
    [Route("api/equipment")]
    public class EquipmentController : ControllerBase
    {
        private static List<Equipment> _equipment = new()
        {
            new() { Id = "CAT320", Name = "Caterpillar 320D", Status = "operational", Hours = 2450 },
            new() { Id = "KOMATSU350", Name = "Komatsu PC350", Status = "operational", Hours = 3120 },
            new() { Id = "VOLVO240", Name = "Volvo EC240B", Status = "operational", Hours = 1890 }
        };

        private static List<ServiceLog> _logs = new();

        [HttpGet]
        public IActionResult GetAll() => Ok(_equipment);

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var eq = _equipment.FirstOrDefault(e => e.Id == id);
            return eq == null ? NotFound() : Ok(eq);
        }

        [HttpGet("{id}/logs")]
        public IActionResult GetLogs(string id) => Ok(_logs.Where(l => l.EquipmentId == id).ToList());

        [HttpPost("{id}/logs")]
        public IActionResult AddLog(string id, [FromBody] ServiceLog log)
        {
            log.EquipmentId = id;
            _logs.Add(log);
            return Ok(log);
        }
    }
}
