using Microsoft.AspNetCore.Mvc;
using HeavyOps.Data.Models;

namespace HeavyOps.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiceLogController : ControllerBase
{
    private static readonly List<ServiceLog> ServiceLogs = new();

    [HttpGet("{equipmentId}")]
    public ActionResult<List<ServiceLog>> GetServiceLogs(string equipmentId)
    {
        var logs = ServiceLogs.Where(l => l.EquipmentId == equipmentId).ToList();
        return Ok(logs);
    }

    [HttpPost("{equipmentId}")]
    public ActionResult<ServiceLog> CreateServiceLog(string equipmentId, [FromBody] ServiceLog log)
    {
        log.Id = Guid.NewGuid().ToString();
        log.EquipmentId = equipmentId;
        log.CreatedAt = DateTime.UtcNow;
        ServiceLogs.Add(log);
        return Ok(log);
    }
}
