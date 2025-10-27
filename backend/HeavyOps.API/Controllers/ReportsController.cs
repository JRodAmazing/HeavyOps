using Microsoft.AspNetCore.Mvc;
using HeavyOps.Data.Models;
using HeavyOps.API.Services;

namespace HeavyOps.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly ReportGeneratorService _reportService;

    public ReportsController(ReportGeneratorService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("{projectId}")]
    public IActionResult GenerateReport(string projectId)
    {
        var pdf = _reportService.GenerateSimplePdf();
        return File(pdf, "application/pdf", $"report_{projectId}.pdf");
    }
}
