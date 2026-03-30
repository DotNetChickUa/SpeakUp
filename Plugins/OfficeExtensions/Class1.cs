using BootlegRealists.Reporting;
using Shared;
using System.ComponentModel;

namespace OfficeExtensions;

[SpeakUpTool]
public class OfficeCommands
{
    [Description("Converts a Word document to PDF")]
    public static async Task WordToPdf(string docxFileName, string pdfFileName)
    {
        await using var docxStream = new FileStream(docxFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        await using var pdfStream = new FileStream(pdfFileName, FileMode.Create, FileAccess.Write, FileShare.Write);
        var docxToPdf = new DocxToPdf();
        var runProperties = new Dictionary<string, string> { ["Title"] = "title", ["UserName"] = "userName" };
        docxToPdf.Execute(docxStream, pdfStream, runProperties);
    }

    [Description("Converts a Word document to PDF 4")]
    public static async Task WordToPdf4(string docxFileName, string pdfFileName)
    {
        
    }
}