using PdfSharpCore.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace ms_documentation.Utils;

public static class HtmlConverter
{
    public static byte[] ConvertHtmlToPdf(string html)
    {
        PdfDocument pdf = PdfGenerator.GeneratePdf(html, PdfSharpCore.PageSize.A4);
        using var stream = new MemoryStream();
        pdf.Save(stream, false);
        stream.Position = 0;
        return stream.ToArray();
    }
}
