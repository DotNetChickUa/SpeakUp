using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Office.Interop.Word;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using Shared;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using System.ComponentModel;
using Spire.Doc;
using Body = DocumentFormat.OpenXml.Wordprocessing.Body;
using Task = System.Threading.Tasks.Task;

namespace OfficeExtensions;

[SpeakUpTool]
public class OfficeCommands
{
    [Description("Converts a Word document to PDF using Spire Doc")]
    public static Task WordToPdfSpireDoc(string docxFileName, string pdfFileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(docxFileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(pdfFileName);

        var docxParts = SplitDocxIntoThreePageStreams(docxFileName, 3);
        var pdfParts = new Dictionary<int, Stream>();

        try
        {
            foreach (var docPart in docxParts.OrderBy(static x => x.Key))
            {
                using Spire.Doc.Document document = new Spire.Doc.Document();
                if (docPart.Value.CanSeek)
                {
                    docPart.Value.Position = 0;
                }

                document.SetCustomFontsFolders("C:\\Projects\\AccuReference\\CollectionsPlatform\\CollectionsPlatformHybrid\\Resources\\Fonts");
                document.LoadFromStream(docPart.Value, FileFormat.Auto);
                document.Replace("{{link}}", "https://localhost:7174/", false, true);

                MemoryStream pdfStream = new MemoryStream();
                document.SaveToStream(pdfStream, FileFormat.PDF);
                pdfStream.Position = 0;
                pdfParts[docPart.Key] = pdfStream;
            }

            MergePdfStreamsToSingleFile(pdfParts, pdfFileName);
        }
        finally
        {
            foreach (var stream in pdfParts.Values)
            {
                stream.Dispose();
            }

            foreach (var stream in docxParts.Values)
            {
                stream.Dispose();
            }
        }

        return Task.CompletedTask;
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

    [Description("Splits a DOCX document into chunks of 3 pages and returns them as a dictionary of streams")]
    public static Dictionary<int, Stream> SplitDocxIntoThreePageStreams(string docxFileName, int pagesPerChunk)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(docxFileName);
        if (pagesPerChunk <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pagesPerChunk), pagesPerChunk, "Value must be greater than 0.");
        }

        byte[] sourceBytes = File.ReadAllBytes(docxFileName);
        List<List<OpenXmlElement>> chunks = new List<List<OpenXmlElement>>();
        OpenXmlElement? sectionProperties = null;

        using (MemoryStream sourceStream = new MemoryStream(sourceBytes, writable: false))
        using (WordprocessingDocument sourceDocument = WordprocessingDocument.Open(sourceStream, false))
        {
            MainDocumentPart mainPart = sourceDocument.MainDocumentPart
                ?? throw new InvalidOperationException("The document does not contain a main document part.");
            Body body = mainPart.Document.Body
                ?? throw new InvalidOperationException("The document does not contain a body.");

            sectionProperties = body.Elements<SectionProperties>().FirstOrDefault()?.CloneNode(true);

            List<OpenXmlElement> currentChunk = new List<OpenXmlElement>();
            int pagesInChunk = 1;

            foreach (OpenXmlElement element in body.Elements().Where(static element => element is not SectionProperties))
            {
                OpenXmlElement clonedElement = element.CloneNode(true);
                currentChunk.Add(clonedElement);

                int breakCount = CountPageBreaks(clonedElement);
                for (int index = 0; index < breakCount; index++)
                {
                    if (pagesInChunk == pagesPerChunk)
                    {
                        TrimTrailingPageBreaks(currentChunk);
                        chunks.Add(currentChunk);
                        currentChunk = new List<OpenXmlElement>();
                        pagesInChunk = 1;
                    }
                    else
                    {
                        pagesInChunk++;
                    }
                }
            }

            if (currentChunk.Count > 0)
            {
                chunks.Add(currentChunk);
            }
        }

        Dictionary<int, Stream> result = new Dictionary<int, Stream>();
        for (int chunkIndex = 0; chunkIndex < chunks.Count; chunkIndex++)
        {
            MemoryStream output = new MemoryStream(sourceBytes.Length);
            output.Write(sourceBytes, 0, sourceBytes.Length);
            output.Position = 0;

            using (WordprocessingDocument splitDocument = WordprocessingDocument.Open(output, true))
            {
                MainDocumentPart splitMainPart = splitDocument.MainDocumentPart
                    ?? throw new InvalidOperationException("The split document does not contain a main document part.");
                Body splitBody = splitMainPart.Document.Body
                    ?? throw new InvalidOperationException("The split document does not contain a body.");

                splitBody.RemoveAllChildren();
                foreach (OpenXmlElement chunkElement in chunks[chunkIndex])
                {
                    splitBody.Append(chunkElement.CloneNode(true));
                }

                if (sectionProperties is not null)
                {
                    splitBody.Append(sectionProperties.CloneNode(true));
                }

                splitMainPart.Document.Save();
            }

            output.Position = 0;
            result[chunkIndex + 1] = output;
        }

        return result;
    }

    private static void MergePdfStreamsToSingleFile(IReadOnlyDictionary<int, Stream> pdfStreams, string outputFileName)
    {
        ArgumentNullException.ThrowIfNull(pdfStreams);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputFileName);

        using PdfDocument mergedDocument = new PdfDocument();

        foreach (int key in pdfStreams.Keys.OrderBy(static x => x))
        {
            Stream pdfStream = pdfStreams[key];
            if (pdfStream.CanSeek)
            {
                pdfStream.Position = 0;
            }

            using PdfDocument sourceDocument = PdfReader.Open(pdfStream, PdfDocumentOpenMode.Import);
            for (int pageIndex = 0; pageIndex < sourceDocument.PageCount; pageIndex++)
            {
                mergedDocument.AddPage(sourceDocument.Pages[pageIndex]);
            }
        }

        mergedDocument.Save(outputFileName);
    }

    private static int CountPageBreaks(OpenXmlElement element)
    {
        int renderedPageBreakCount = element.Descendants<LastRenderedPageBreak>().Count();
        int manualPageBreakCount = element
            .Descendants<DocumentFormat.OpenXml.Wordprocessing.Break>()
            .Count(static breakElement => breakElement.Type?.Value == BreakValues.Page);

        int sectionBreakCount = element
            .Descendants<SectionProperties>()
            .Count(static sectionProperties =>
            {
                SectionMarkValues sectionType = sectionProperties.GetFirstChild<SectionType>()?.Val?.Value
                    ?? SectionMarkValues.NextPage;

                return sectionType == SectionMarkValues.NextPage
                    || sectionType == SectionMarkValues.OddPage
                    || sectionType == SectionMarkValues.EvenPage;
            });

        return renderedPageBreakCount + manualPageBreakCount + sectionBreakCount;
    }

    private static void TrimTrailingPageBreaks(List<OpenXmlElement> chunkElements)
    {
        if (chunkElements.Count == 0)
        {
            return;
        }

        OpenXmlElement lastElement = chunkElements[^1];

        foreach (var breakElement in lastElement
                     .Descendants<DocumentFormat.OpenXml.Wordprocessing.Break>()
                     .Where(static breakElement => breakElement.Type?.Value == BreakValues.Page)
                     .ToList())
        {
            breakElement.Remove();
        }

        foreach (var renderedBreak in lastElement.Descendants<LastRenderedPageBreak>().ToList())
        {
            renderedBreak.Remove();
        }
    }
}