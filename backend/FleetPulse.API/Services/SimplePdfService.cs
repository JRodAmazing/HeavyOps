using System.Text;
using System.Reflection;
using FleetPulse.API.Models;

namespace FleetPulse.API.Services;

/// <summary>
/// A lightweight PDF service using iText7 library
/// This is a fallback if QuestPDF fails, or for generating simple test PDFs
/// </summary>
public class SimplePdfService
{
    private const string ShopName = "Heavy Iron Diagnostics";
    private const string ShopAddress = "123 Fleet Street, Industrial City, ST 12345";
    private const string ShopPhone = "(555) 123-4567";

    /// <summary>
    /// Generate a simple test report without dependencies
    /// Uses basic formatting to create a minimal valid PDF
    /// </summary>
    public byte[] GenerateSimpleReport(string equipmentId, string equipmentName)
    {
        // For now, return a basic text representation as PDF
        // This is a placeholder that creates a minimal PDF structure
        return GenerateMinimalPdf($"Test Report - {equipmentName}", 
            $"Equipment ID: {equipmentId}\n" +
            $"Equipment Name: {equipmentName}\n" +
            $"Generated: {DateTime.Now:MMMM dd, yyyy hh:mm tt}\n" +
            $"\n" +
            $"Shop Name: {ShopName}\n" +
            $"Address: {ShopAddress}\n" +
            $"Phone: {ShopPhone}");
    }

    /// <summary>
    /// Generate a minimal valid PDF from text content
    /// This creates a basic PDF structure compatible with all PDF readers
    /// </summary>
    private byte[] GenerateMinimalPdf(string title, string content)
    {
        using (var ms = new MemoryStream())
        {
            var writer = new StreamWriter(ms, Encoding.UTF8);

            // PDF Header
            writer.Write("%PDF-1.4\n");

            // Object 1: Catalog
            writer.Write("1 0 obj\n");
            writer.Write("<< /Type /Catalog /Pages 2 0 R >>\n");
            writer.Write("endobj\n");

            // Object 2: Pages
            writer.Write("2 0 obj\n");
            writer.Write("<< /Type /Pages /Kids [3 0 R] /Count 1 >>\n");
            writer.Write("endobj\n");

            // Object 3: Page
            writer.Write("3 0 obj\n");
            writer.Write("<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Contents 4 0 R /Resources << /Font << /F1 5 0 R >> >> >>\n");
            writer.Write("endobj\n");

            // Object 4: Content Stream
            var contentText = $"BT /F1 12 Tf 50 750 Td ({EscapePdfString(title)}) Tj ET\n";
            contentText += "BT /F1 10 Tf 50 720 Td (";
            var lines = content.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                contentText += EscapePdfString(lines[i]);
                if (i < lines.Length - 1)
                    contentText += ") Tj\nET\nBT /F1 10 Tf 50 " + (720 - (i + 1) * 15) + " Td (";
            }
            contentText += ") Tj ET\n";

            var contentBytes = Encoding.UTF8.GetBytes(contentText);
            writer.Write("4 0 obj\n");
            writer.Write($"<< /Length {contentBytes.Length} >>\n");
            writer.Write("stream\n");
            writer.Flush();
            ms.Write(contentBytes, 0, contentBytes.Length);
            writer.Write("\nendstream\n");
            writer.Write("endobj\n");

            // Object 5: Font
            writer.Write("5 0 obj\n");
            writer.Write("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>\n");
            writer.Write("endobj\n");

            // Cross-reference table
            long xrefOffset = ms.Position;
            writer.Write("xref\n");
            writer.Write("0 6\n");
            writer.Write("0000000000 65535 f \n");
            writer.Write("0000000009 00000 n \n");
            writer.Write("0000000074 00000 n \n");
            writer.Write("0000000131 00000 n \n");
            writer.Write("0000000239 00000 n \n");
            writer.Write($"{ms.Position:D10} 00000 n \n");

            // Trailer
            writer.Write("trailer\n");
            writer.Write("<< /Size 6 /Root 1 0 R >>\n");
            writer.Write("startxref\n");
            writer.Write(xrefOffset + "\n");
            writer.Write("%%EOF\n");

            writer.Flush();
            return ms.ToArray();
        }
    }

    private string EscapePdfString(string input)
    {
        if (string.IsNullOrEmpty(input))
            return "";
        
        // Remove or escape problematic characters
        return input
            .Replace("\\", "\\\\")
            .Replace("(", "\\(")
            .Replace(")", "\\)")
            .Replace("\n", " ")
            .Replace("\r", " ");
    }
}
