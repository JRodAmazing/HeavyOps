using Microsoft.AspNetCore.Mvc;
using FleetPulse.API.Services;
using FleetPulse.API.Models;  // ← ADD THIS LINE
using System.Collections.Generic;

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
            if (frame == null)
                return BadRequest("Frame cannot be null");

            _streamService.AddFrame(frame);
            return Ok(new { message = "Frame ingested", frameId = frame.Id });
        }

        [HttpGet("{equipmentId}/latest")]
        public IActionResult GetLatestFrame(string equipmentId)
        {
            var frame = _streamService.GetLatestFrame(equipmentId);
            if (frame == null)
                return NotFound($"No frames for equipment {equipmentId}");

            return Ok(frame);
        }

        [HttpGet("{equipmentId}/recent")]
        public IActionResult GetRecentFrames(string equipmentId)
        {
            var frames = _streamService.GetRecentFrames(equipmentId, 60);
            return Ok(frames);
        }
    }
}