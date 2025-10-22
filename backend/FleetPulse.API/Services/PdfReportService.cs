using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using FleetPulse.API.Models;

namespace FleetPulse.API.Services
{
    public class PdfReportService
    {
        public byte[] GenerateEquipmentReport(Equipment equipment, List<ServiceLog> logs)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var document = Document.Create(container =>
                container.Page(page =>
                {
                    page.Size(new(8.5f, 11f));
                    page.Margin(20);
                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Report: {equipment.Name}").FontSize(20).Bold();
                        col.Item().Text($"ID: {equipment.Id}").FontSize(12);
                        col.Item().Text($"Hours: {equipment.Hours}").FontSize(12);
                    });
                })
            );
            return document.GeneratePdf();
        }
    }
}
