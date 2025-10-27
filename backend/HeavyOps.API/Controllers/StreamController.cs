using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace HeavyOps.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StreamController : ControllerBase
    {
        private static Dictionary<string, List<DiagnosticFrame>> _frames = new();

        [HttpPost("ingest")]
        public IActionResult IngestFrame([FromBody] DiagnosticFrame frame)
        {
            if (frame == null)
                return BadRequest("Invalid frame");

            if (!_frames.ContainsKey(frame.EquipmentId))
            {
                _frames[frame.EquipmentId] = new List<DiagnosticFrame>();
            }

            _frames[frame.EquipmentId].Add(frame);

            // Keep only last 60 frames
            if (_frames[frame.EquipmentId].Count > 60)
            {
                _frames[frame.EquipmentId].RemoveAt(0);
            }

            return Ok();
        }

        [HttpGet("{equipmentId}/latest")]
        public IActionResult GetLatestFrame(string equipmentId)
        {
            if (!_frames.ContainsKey(equipmentId) || _frames[equipmentId].Count == 0)
                return NotFound();

            return Ok(_frames[equipmentId].Last());
        }

        [HttpGet("{equipmentId}/recent")]
        public IActionResult GetRecentFrames(string equipmentId)
        {
            if (!_frames.ContainsKey(equipmentId))
                return Ok(new List<DiagnosticFrame>());

            return Ok(_frames[equipmentId]);
        }
    }

    public class DiagnosticFrame
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string EquipmentId { get; set; }
        public DateTime Timestamp { get; set; }
        public string CanId { get; set; }
        public string Data { get; set; }
    }
}