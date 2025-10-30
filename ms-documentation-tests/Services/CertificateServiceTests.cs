using System.Text;
using ms_documentation_tests.Utils;
using ms_documentation.Services;
using UglyToad.PdfPig;

namespace ms_documentation_tests.Services;

public class CertificateServiceTests()
{
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
    [Fact]
    public void CanGeneratePDFCertificate()
    {
        var alumno = MockDataFactory.CreateAlumno();
        var certificateFile = CertificateService.GeneratePDF(alumno);

        Assert.NotNull(certificateFile);
        Assert.True(certificateFile.Length > 0);

        var fullText = GetPDFText(certificateFile);

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
}