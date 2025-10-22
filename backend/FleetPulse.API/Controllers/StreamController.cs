using Microsoft.AspNetCore.Mvc;
using FleetPulse.API.Models;
using FleetPulse.API.Services;

namespace FleetPulse.API.Controllers
{
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
            if (frame == null) return BadRequest();
            _streamService.AddFrame(frame);
            return Ok(new { message = "Frame ingested", frameId = frame.Id });
        }

        [HttpGet("{equipmentId}/latest")]
        public IActionResult GetLatestFrame(string equipmentId)
        {
            var frame = _streamService.GetLatestFrame(equipmentId);
            return frame == null ? NotFound() : Ok(frame);
        }

        [HttpGet("{equipmentId}/recent")]
        public IActionResult GetRecentFrames(string equipmentId) => Ok(_streamService.GetRecentFrames(equipmentId, 60));
    }
}
