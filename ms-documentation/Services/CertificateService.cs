using ms_documentation.Models;
using ms_documentation.Utils;

namespace ms_documentation.Services;

public class CertificateService()
{
    private static string GetTemplateText(string fileName)
    {
        var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", fileName);
        return File.ReadAllText(templatePath);
    }
    private static string FormatFileTextToAlumno(string text,Alumno alumno)
    {
        return text.Replace("{{fecha}}", DateTime.Now.ToLongDateString())
                   .Replace("{{alumno.nombre}}", alumno.Nombre)
                   .Replace("{{alumno.apellido}}", alumno.Apellido)
                   .Replace("{{alumno.tipo_documento.sigla}}",alumno.TipoDocumento.ToString())
                   .Replace("{{alumno.nrodocumento}}",alumno.NroDocumento)
                   .Replace("{{alumno.nro_legajo}}",alumno.NroLegajo.ToString())
                   .Replace("{{especialidad.nombre}}",alumno.Especialidad.Nombre)
                   .Replace("{{facultad.nombre}}",alumno.Especialidad.Facultad.Nombre)
                   .Replace("{{universidad.nombre}}",alumno.Especialidad.Facultad.Universidad.Nombre)
                   .Replace("{{facultad.ciudad}}",alumno.Especialidad.Facultad.Ciudad)
                   ;
    }
    public static byte[] GeneratePDF(Alumno alumno)
    {
        var templateHTML = GetTemplateText("certificado_pdf.html");
        var formattedHTML = FormatFileTextToAlumno(templateHTML, alumno);
        return HtmlConverter.ConvertHtmlToPdf(formattedHTML);
    }
}