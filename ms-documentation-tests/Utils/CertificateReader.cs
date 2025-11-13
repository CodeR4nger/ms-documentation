using System.Text;
using System.Xml.Linq;
using AODL.Document.Content;
using AODL.Document.TextDocuments;
using ms_documentation.Models;
using UglyToad.PdfPig;
using Xceed.Words.NET;

namespace ms_documentation_tests.Utils;

public static class CertificateReader
{
    public static string GetPDFText(byte[] data)
    {
        using var pdfStream = new MemoryStream(data);
        using var document = PdfDocument.Open(pdfStream);
        var pdfText = new StringBuilder();
        foreach (var page in document.GetPages())
        {
            pdfText.AppendLine(page.Text);
        }

        return pdfText.ToString();
    }
    public static string GetOdtText(byte[] data)
    {
        using var odtStream = new MemoryStream(data);
        string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".odt");
        try
        {
            using (var fileStream = File.Create(tempPath))
            {
                odtStream.Seek(0, SeekOrigin.Begin);
                odtStream.CopyTo(fileStream);
            }

            var sb = new StringBuilder();
            using (var doc = new TextDocument())
            {
                doc.Load(tempPath);

                XElement stylesPart = XElement.Parse(doc.DocumentStyles.Styles.OuterXml);
                string stylesText = string.Join(
                    "\r\n",
                    stylesPart
                        .Descendants()
                        .Where(x => x.Name.LocalName == "header" || x.Name.LocalName == "footer")
                        .Select(y => y.Value)
                );

                var mainPart = doc.Content.Cast<IContent>();
                var mainText = string.Join("\r\n", mainPart.Select(x => x.Node.InnerText));

                sb.Append(stylesText);
                sb.AppendLine();
                sb.Append(mainText);
            }

            return sb.ToString();
        }
        finally
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }
    public static string GetDocxText(byte[] data)
    {
        using var memoryStream = new MemoryStream(data);
        using var wordDocument = DocX.Load(memoryStream);
        return wordDocument.Text;
    }

    public static void AssertCertificateText(string? fullText,Alumno alumno)
    {
        Assert.NotNull(fullText);
        var expectedValues = new[]
        {
            alumno.Nombre,
            alumno.Apellido,
            alumno.NroDocumento,
            alumno.NroLegajo.ToString(),
            alumno.TipoDocumento.ToString(),
            alumno.Especialidad.Nombre,
            alumno.Especialidad.Facultad.Nombre,
            alumno.Especialidad.Facultad.Universidad.Nombre
        };

        static string Normalize(string s) => new([.. s.Where(c => !char.IsWhiteSpace(c))]);
        var normalizedText = Normalize(fullText);

        foreach (var value in expectedValues)
        {
            Assert.Contains(Normalize(value), normalizedText);
        }
    }
}