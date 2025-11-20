using System.IO.Compression;
using System.Text;
using System.Xml.Linq;
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
    public static string GetOdtText(byte[] odtFileBytes)
    {
        try
        {
            using (var memoryStream = new MemoryStream(odtFileBytes))
            {
                using (var zipArchive = new ZipArchive(memoryStream))
                {
                    var contentFile = zipArchive.GetEntry("content.xml");
                    if (contentFile != null)
                    {
                        using (var stream = contentFile.Open())
                        {
                            XDocument doc = XDocument.Load(stream);
                            StringBuilder sb = new StringBuilder();

                            foreach (var paragraph in doc.Descendants("{urn:oasis:names:tc:opendocument:xmlns:text:1.0}p"))
                            {
                                foreach (var textElement in paragraph.Descendants("{urn:oasis:names:tc:opendocument:xmlns:text:1.0}span"))
                                {
                                    sb.Append(textElement.Value);
                                }
                                sb.AppendLine();
                            }

                            return sb.ToString();
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("No se encontró el archivo content.xml dentro del archivo ODT.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return $"Error al procesar el archivo ODT: {ex.Message}";
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