using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using FleetPulse.API.Models;

namespace FleetPulse.API.Services;

public class PdfReportService
{
    static PdfReportService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateEquipmentReport(Equipment equipment)
    {
        try
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);

                    page.Content().Column(column =>
                    {
                        // Header
                        column.Item().PaddingBottom(20).Row(row =>
                        {
                            row.RelativeItem().Column(innerColumn =>
                            {
                                innerColumn.Item().Text("EQUIPMENT REPORT").FontSize(24).Bold();
                                innerColumn.Item().Text($"Equipment ID: {equipment.Id}").FontSize(12);
                            });

                            row.RelativeItem().AlignRight().Column(innerColumn =>
                            {
                                innerColumn.Item().Text("Generated").FontSize(10).Bold();
                                innerColumn.Item().Text(DateTime.Now.ToString("yyyy-MM-dd HH:mm")).FontSize(10);
                            });
                        });

                        // Divider
                        column.Item().PaddingBottom(20).LineHorizontal(1);

                        // Equipment Info Section
                        column.Item().PaddingBottom(15).Column(equipSection =>
                        {
                            equipSection.Item().Text("Equipment Information").FontSize(14).Bold();
                            equipSection.Item().PaddingBottom(10).Text("");

                            equipSection.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(2);
                                });

                                table.Cell().Text("Name:").Bold();
                                table.Cell().Text(equipment.Name ?? "N/A");

                                table.Cell().Text("Status:").Bold();
                                table.Cell().Text(equipment.Status ?? "N/A");

                                table.Cell().Text("Hours:").Bold();
                                table.Cell().Text(equipment.Hours.ToString());
                            });
                        });

                        // Service History Section
                        column.Item().PaddingBottom(15).Column(serviceSection =>
                        {
                            serviceSection.Item().Text("Service History").FontSize(14).Bold();
                            serviceSection.Item().PaddingBottom(10).Text("");

                            if (equipment.ServiceLogs != null && equipment.ServiceLogs.Any())
                            {
                                serviceSection.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(1.2f);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(1.5f);
                                        columns.RelativeColumn(1);
                                    });

                                    // Header
                                    table.Header(header =>
                                    {
                                        header.Cell().Background("#E8E8E8").Padding(5).Text("Date").Bold();
                                        header.Cell().Background("#E8E8E8").Padding(5).Text("Notes").Bold();
                                        header.Cell().Background("#E8E8E8").Padding(5).Text("Labor Cost").Bold();
                                        header.Cell().Background("#E8E8E8").Padding(5).Text("Hours").Bold();
                                    });

                                    // Rows
                                    foreach (var log in equipment.ServiceLogs)
                                    {
                                        table.Cell().Padding(5).Text(log.DatePerformed.ToShortDateString());
                                        table.Cell().Padding(5).Text(log.Notes ?? "N/A");
                                        table.Cell().Padding(5).Text($"${log.LaborCost:F2}");
                                        table.Cell().Padding(5).Text(log.HoursAtService.ToString());
                                    }
                                });
                            }
                            else
                            {
                                serviceSection.Item().Padding(20).Text("No service logs recorded").Italic();
                            }
                        });

                        // Footer
                        column.Item().PaddingTop(30).BorderTop(1).PaddingTop(10).AlignCenter().Text(text =>
                        {
                            text.Span("Report generated on ");
                            text.Span(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).Bold();
                        });
                    });
                });
            });

            return document.GeneratePdf();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"PDF Generation Error: {ex.GetType().Name}");
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
            throw;
        }
    }

    public byte[] GenerateSimplePdf(Equipment equipment)
    {
        try
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Content().Column(column =>
                    {
                        column.Item().Text($"Equipment Report: {equipment.Name}").FontSize(20).Bold();
                        column.Item().PaddingTop(10).Text($"ID: {equipment.Id}");
                        column.Item().PaddingTop(5).Text($"Status: {equipment.Status}");
                        column.Item().PaddingTop(5).Text($"Hours: {equipment.Hours}");
                        column.Item().PaddingTop(20).Text($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}").FontSize(10).Italic();
                    });
                });
            });

            return document.GeneratePdf();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Simple PDF Generation Error: {ex.GetType().Name}");
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
            throw;
        }
    }
}
