using Microsoft.AspNetCore.Mvc;
using FleetPulse.API.Models;
using FleetPulse.API.Services;

namespace FleetPulse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly PdfReportService _pdfService;

    public ReportsController(PdfReportService pdfService)
    {
        _pdfService = pdfService;
    }

    /// <summary>
    /// Test endpoint - simple PDF generation without QuestPDF
    /// </summary>
    [HttpGet("{equipmentId}/generate-simple")]
    public IActionResult GenerateSimpleReport(string equipmentId)
    {
        try
        {
            var equipment = EquipmentController.GetEquipmentList().FirstOrDefault(e => e.Id == equipmentId);
            
            if (equipment == null)
            {
                return NotFound(new { message = $"Equipment with ID '{equipmentId}' not found." });
            }

            var simplePdfService = new SimplePdfService();
            var pdfBytes = simplePdfService.GenerateSimpleReport(equipment.Id, equipment.Name);

            return File(
                pdfBytes,
                "application/pdf",
                $"Test_Report_{equipment.Id}_{DateTime.Now:yyyy-MM-dd_HHmmss}.pdf"
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Simple PDF Error: {ex.Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
            return StatusCode(500, new { message = ex.Message, stackTrace = ex.StackTrace });
        }
    }

    /// <summary>
    /// Generate a PDF service report for a specific equipment
    /// </summary>
    [HttpGet("{equipmentId}/generate")]
    public IActionResult GenerateReport(string equipmentId)
    {
        try
        {
            // Get the equipment from the static list
            var equipment = EquipmentController.GetEquipmentList().FirstOrDefault(e => e.Id == equipmentId);
            
            if (equipment == null)
            {
                return NotFound(new { message = $"Equipment with ID '{equipmentId}' not found." });
            }

            // Generate the PDF
            var pdfBytes = _pdfService.GenerateEquipmentReport(equipment);

            // Return the PDF file
            return File(
                pdfBytes,
                "application/pdf",
                $"Service_Report_{equipment.Id}_{DateTime.Now:yyyy-MM-dd_HHmmss}.pdf"
            );
        }
        catch (Exception ex)
        {
            // Log the full exception for debugging
            Console.WriteLine($"PDF Generation Error: {ex.GetType().Name}");
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
            
            return StatusCode(500, new { 
                message = "Error generating report", 
                error = ex.Message,
                stackTrace = ex.StackTrace
            });
        }
    }
}
