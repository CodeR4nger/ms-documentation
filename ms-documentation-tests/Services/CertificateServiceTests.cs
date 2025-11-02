using System.Text;
using ms_documentation_tests.Utils;
using ms_documentation.Services;
using UglyToad.PdfPig;
using Xceed.Words.NET;
using ms_documentation.Models;


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
}