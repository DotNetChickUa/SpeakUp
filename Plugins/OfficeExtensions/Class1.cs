using BootlegRealists.Reporting;
using Microsoft.Office.Interop.Word;
using Shared;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Pdf;
using System.ComponentModel;
using Winnovative.Pdf.Next;
using Document = Microsoft.Office.Interop.Word.Document;
using Task = System.Threading.Tasks.Task;
using WordToPdfConverter = WnvWordToPdf.WordToPdfConverter;

namespace OfficeExtensions;

[SpeakUpTool]
public class OfficeCommands
{
    [Description("Converts a Word document to PDF using DocxToPdf")]
    public static async Task WordToPdf(string docxFileName, string pdfFileName)
    {
        await using var docxStream = new FileStream(docxFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        await using var pdfStream = new FileStream(pdfFileName, FileMode.Create, FileAccess.Write, FileShare.Write);
        var docxToPdf = new DocxToPdf();
        var runProperties = new Dictionary<string, string> { ["Title"] = "title", ["UserName"] = "userName" };
        docxToPdf.Execute(docxStream, pdfStream, runProperties);
    }

    [Description("Converts a Word document to PDF using Spire Doc")]
    public static async Task WordToPdfSpireDoc(string docxFileName, string pdfFileName)
    {
        Spire.Doc.Document document = new Spire.Doc.Document();
        document.LoadFromFile(docxFileName);
        document.Replace("{{title}}", "title", false, true);
        document.SaveToFile(pdfFileName, Spire.Doc.FileFormat.PDF);
    }

    [Description("Converts a Word document to PDF using WordToPdfConverter")]
    public static async Task WordToPdfWordToPdfConverter(string docxFileName, string pdfFileName)
    {
        WordToPdfConverter wordToPdfConverter = new WordToPdfConverter();
        wordToPdfConverter.LicenseKey = "DYOSgpaRgpKClIySgpGTjJOQjJubm5uCkg==";

        wordToPdfConverter.PdfDocumentOptions.ShowHeader = true;
        wordToPdfConverter.PdfDocumentOptions.ShowFooter = true;
        byte[] outPdfBuffer = wordToPdfConverter.ConvertWordFile(docxFileName);
        await File.WriteAllBytesAsync(pdfFileName, outPdfBuffer);
    }

    [Description("Converts a Word document to PDF using Next WordToPdfConverter")]
    public static async Task WordToPdfNextWordToPdfConverter(string docxFileName, string pdfFileName)
    {
        //Licensing.LicenseKey = "DYOSgpaRgpKClIySgpGTjJOQjJubm5uCkg==";
        var wordToPdfConverter = new Winnovative.Pdf.Next.WordToPdfConverter();
        await wordToPdfConverter.ConvertToPdfFileAsync(docxFileName, pdfFileName);
    }

    [Description("Converts a Word document to PDF using COM")]
    public static void WordToPdfCOM(string docxFileName, string pdfFileName)
    {
        Application appWord = new Application();
        var wordDocument = appWord.Documents.Open(docxFileName);
        wordDocument.ExportAsFixedFormat(pdfFileName, WdExportFormat.wdExportFormatPDF);
        wordDocument.Close();
        appWord.Quit();
    }

    [Description("Converts a Word document to PDF using Syncfusion")]
    public static async Task WordToPdfSyncfusion(string docxFileName, string pdfFileName, string? licenseKey = null)
    {
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(licenseKey);
        await using FileStream docStream = new FileStream(docxFileName, FileMode.Open, FileAccess.Read);
        using WordDocument wordDocument = new WordDocument(docStream, FormatType.Docx);
        using DocIORenderer render = new DocIORenderer();
        using var pdfDocument = render.ConvertToPDF(wordDocument);
        await using FileStream stream = new FileStream(pdfFileName, FileMode.Create, FileAccess.Write);
        pdfDocument.Save(stream);
    }
}