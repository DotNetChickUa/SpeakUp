using BootlegRealists.Reporting;
using Shared;
using System.ComponentModel;
using WnvWordToPdf;

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
        Spire.Doc.Document document = new Spire.Doc.Document();
        document.LoadFromFile(docxFileName);
        document.Replace("{{title}}", "title", false, true);
        document.SaveToFile(pdfFileName, Spire.Doc.FileFormat.PDF);
    }

    [Description("Converts a Word document to PDF 5")]
    public static async Task WordToPdf5(string docxFileName, string pdfFileName)
    {
        WordToPdfConverter wordToPdfConverter = new WordToPdfConverter();
        wordToPdfConverter.LicenseKey = "DYOSgpaRgpKClIySgpGTjJOQjJubm5uCkg==";

        wordToPdfConverter.PdfDocumentOptions.ShowHeader = true;
        wordToPdfConverter.PdfDocumentOptions.ShowFooter = true;
        byte[] outPdfBuffer = wordToPdfConverter.ConvertWordFile(docxFileName);
        await System.IO.File.WriteAllBytesAsync(pdfFileName, outPdfBuffer);
    }
}