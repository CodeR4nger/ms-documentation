using System.Text;
using ms_documentation_tests.Utils;
using ms_documentation.Services;
using UglyToad.PdfPig;
using Xceed.Words.NET;
using ms_documentation.Models;
using System.Xml.Linq;
using AODL.Document.TextDocuments;
using AODL.Document.Content;


namespace ms_documentation_tests.Services;

public class CertificateServiceTests()
{
    private static bool IsFileEmpty(byte[] file)
    {
        return file == null || file.Length <= 0;
    }
    private static string GetPDFText(byte[] pdfData)
    {
        using var pdfStream = new MemoryStream(pdfData);
        using var document = PdfDocument.Open(pdfStream);
        var pdfText = new StringBuilder();
        foreach (var page in document.GetPages())
        {
            pdfText.AppendLine(page.Text);
        }

        return pdfText.ToString();
    }
    private static string GetTextFromOdt(byte[] odtData)
    {
        using var odtStream = new MemoryStream(odtData);
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

    private static void AssertCertificateText(string? fullText,Alumno alumno)
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
            alumno.Especialidad.Facultad.Ciudad,
            alumno.Especialidad.Facultad.Universidad.Nombre
        };

        static string Normalize(string s) => new([.. s.Where(c => !char.IsWhiteSpace(c))]);
        var normalizedText = Normalize(fullText);

        foreach (var value in expectedValues)
        {
            Assert.Contains(Normalize(value), normalizedText);
        }
    }
    [Fact]
    public void CanGeneratePDFCertificate()
    {
        var alumno = MockDataFactory.CreateAlumno();
        var certificateFile = CertificateService.GeneratePDF(alumno);

        Assert.False(IsFileEmpty(certificateFile));

        var fullText = GetPDFText(certificateFile);

        AssertCertificateText(fullText, alumno);
    }
    [Fact]
    public void CanGenerateDocxCertificate()
    {
        var alumno = MockDataFactory.CreateAlumno();
        var certificateFile = CertificateService.GenerateDocx(alumno);

        Assert.False(IsFileEmpty(certificateFile));

        using var memoryStream = new MemoryStream(certificateFile);
        using var wordDocument = DocX.Load(memoryStream);
        var documentText = wordDocument.Text;

        AssertCertificateText(documentText, alumno);
    }
    [Fact]
    public void CanGenerateOdtCertificate()
    {
        var alumno = MockDataFactory.CreateAlumno();
        var certificateFile = CertificateService.GenerateOdt(alumno);

        Assert.False(IsFileEmpty(certificateFile));

        var documentText = GetTextFromOdt(certificateFile);

        AssertCertificateText(documentText, alumno);
    }
}