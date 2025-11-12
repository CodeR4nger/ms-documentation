using ms_documentation_tests.Utils;
using ms_documentation.Services;

namespace ms_documentation_tests.Services;

public class CertificateServiceTests()
{
    private static bool IsFileEmpty(byte[] file)
    {
        return file == null || file.Length <= 0;
    }
    [Fact]
    public void CanGeneratePDFCertificate()
    {
        var alumno = MockDataFactory.CreateAlumno();
        var certificateFile = CertificateService.GeneratePDF(alumno);

        Assert.False(IsFileEmpty(certificateFile));

        var fullText = CertificateReader.GetPDFText(certificateFile);

        CertificateReader.AssertCertificateText(fullText, alumno);
    }
    [Fact]
    public void CanGenerateDocxCertificate()
    {
        var alumno = MockDataFactory.CreateAlumno();
        var certificateFile = CertificateService.GenerateDocx(alumno);

        Assert.False(IsFileEmpty(certificateFile));

        var documentText = CertificateReader.GetDocxText(certificateFile);

        CertificateReader.AssertCertificateText(documentText, alumno);
    }
    [Fact]
    public void CanGenerateOdtCertificate()
    {
        var alumno = MockDataFactory.CreateAlumno();
        var certificateFile = CertificateService.GenerateOdt(alumno);

        Assert.False(IsFileEmpty(certificateFile));

        var documentText = CertificateReader.GetOdtText(certificateFile);

        CertificateReader.AssertCertificateText(documentText, alumno);
    }
}