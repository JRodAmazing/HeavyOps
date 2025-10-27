using HeavyOps.Data.Models;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;

namespace HeavyOps.API.Services;

public class ReportGeneratorService
{
    public byte[] GenerateProjectReport(Project project)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(20);
                page.Content().Column(col =>
                {
                    col.Item().Text($"Project: {project.Name}").FontSize(20).Bold();
                    col.Item().Text($"Location: {project.Location}");
                    col.Item().Text($"Budget: ${project.Budget}");
                });
            });
        });

        return document.GeneratePdf();
    }

    public byte[] GenerateSimplePdf()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(20);
                page.Content().Text("HeavyOps Report").FontSize(24).Bold();
            });
        });

        return document.GeneratePdf();
    }
}

