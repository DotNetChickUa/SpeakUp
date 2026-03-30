using Microsoft.Office.Interop.Word;
using Shared;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using System.ComponentModel;
using Task = System.Threading.Tasks.Task;

namespace OfficeExtensions;

[SpeakUpTool]
public class OfficeCommands
{
    [Description("Converts a Word document to PDF using Spire Doc")]
    public static async Task WordToPdfSpireDoc(string docxFileName, string pdfFileName)
    {
        Spire.Doc.Document document = new Spire.Doc.Document();
        document.LoadFromFile(docxFileName);
        document.Replace("{{link}}", "https://localhost:7174/", false, true);
        document.SaveToFile(pdfFileName, Spire.Doc.FileFormat.PDF);
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
        wordDocument.Replace("{{link}}", "https://localhost:7174/", false, true);
        using DocIORenderer render = new DocIORenderer();
        using var pdfDocument = render.ConvertToPDF(wordDocument);
        await using FileStream stream = new FileStream(pdfFileName, FileMode.Create, FileAccess.Write);
        pdfDocument.Save(stream);
    }
}