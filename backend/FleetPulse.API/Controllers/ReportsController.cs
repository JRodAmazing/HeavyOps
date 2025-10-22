using Microsoft.AspNetCore.Mvc;
using FleetPulse.API.Models;
using FleetPulse.API.Services;

namespace FleetPulse.API.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly PdfReportService _pdfService;
        private static List<Equipment> _equipment = new()
        {
            new() { Id = "CAT320", Name = "Caterpillar 320D", Status = "operational", Hours = 2450 },
            new() { Id = "KOMATSU350", Name = "Komatsu PC350", Status = "operational", Hours = 3120 },
            new() { Id = "VOLVO240", Name = "Volvo EC240B", Status = "operational", Hours = 1890 }
        };

        public ReportsController(PdfReportService pdfService)
        {
            _pdfService = pdfService;
        }

        [HttpGet("{id}/generate")]
        public IActionResult GenerateReport(string id)
        {
            var eq = _equipment.FirstOrDefault(e => e.Id == id);
            if (eq == null) return NotFound();
            var pdf = _pdfService.GenerateEquipmentReport(eq, new List<ServiceLog>());
            return File(pdf, "application/pdf", $"{id}_report.pdf");
        }
    }
}
