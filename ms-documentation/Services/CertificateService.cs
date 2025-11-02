using ms_documentation.Models;
using ms_documentation.Utils;
using Xceed.Words.NET;

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
    public static byte[] GenerateDocx(Alumno alumno)
    {
        var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "certificado_docx.docx");
        using var memoryStream = new MemoryStream();
        
        using (var fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
        {
            fileStream.CopyTo(memoryStream);
        }

        memoryStream.Position = 0;
        using (var doc = DocX.Load(memoryStream))
        {
            #pragma warning disable CS0618
            doc.ReplaceText("{{fecha}}", DateTime.Now.ToLongDateString());
            doc.ReplaceText("{{alumno.nombre}}", alumno.Nombre);
            doc.ReplaceText("{{alumno.apellido}}", alumno.Apellido);
            doc.ReplaceText("{{alumno.tipo_documento.sigla}}", alumno.TipoDocumento.ToString());
            doc.ReplaceText("{{alumno.nrodocumento}}", alumno.NroDocumento);
            doc.ReplaceText("{{alumno.nro_legajo}}", alumno.NroLegajo.ToString());
            doc.ReplaceText("{{especialidad.nombre}}", alumno.Especialidad.Nombre);
            doc.ReplaceText("{{facultad.nombre}}", alumno.Especialidad.Facultad.Nombre);
            doc.ReplaceText("{{universidad.nombre}}", alumno.Especialidad.Facultad.Universidad.Nombre);
            doc.ReplaceText("{{facultad.ciudad}}", alumno.Especialidad.Facultad.Ciudad);
            #pragma warning restore CS0618
            using var outputStream = new MemoryStream();
            doc.SaveAs(outputStream);
            return outputStream.ToArray();
        }
    }
}