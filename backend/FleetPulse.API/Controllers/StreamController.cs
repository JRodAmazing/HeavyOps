using FleetPulse.API.Models;
using FleetPulse.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace FleetPulse.API.Controllers;

[ApiController]
[Route("api/stream")]
public class StreamController : ControllerBase
{
    private readonly DiagnosticStreamService _streamService;

    public StreamController(DiagnosticStreamService streamService)
    {
        _streamService = streamService;
    }

    [HttpPost("ingest")]
    public IActionResult IngestFrame([FromBody] DiagnosticFrame frame)
    {
        if (string.IsNullOrEmpty(frame.EquipmentId) || string.IsNullOrEmpty(frame.CanId))
            return BadRequest("Missing EquipmentId or CanId");

        _streamService.IngestFrame(frame);
        
        var parsed = _streamService.ParseJ1939Frame(frame.Data);
        
        return Ok(new
        {
            message = "Frame ingested",
            frameId = frame.Id,
            parsed
        });
    }

    [HttpGet("{equipmentId}/latest")]
    public IActionResult GetLatestFrame(string equipmentId)
    {
        var frame = _streamService.GetLatestFrame(equipmentId);
        if (frame == null)
            return NotFound();

        var parsed = _streamService.ParseJ1939Frame(frame.Data);
        
        return Ok(new { frame, parsed });
    }

    [HttpGet("{equipmentId}/recent")]
    public IActionResult GetRecentFrames(string equipmentId, [FromQuery] int count = 60)
    {
        var frames = _streamService.GetRecentFrames(equipmentId, count);
        return Ok(frames);
    }
}
