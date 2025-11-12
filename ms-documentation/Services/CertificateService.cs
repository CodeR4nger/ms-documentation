using System.IO.Compression;
using System.Text;
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
    public static byte[] GenerateOdt(Alumno alumno)
    {
        // Template file path
        var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "certificado_odt.odt");

        // Read the whole template into memory
        byte[] templateBytes = File.ReadAllBytes(templatePath);

        using var inputStream = new MemoryStream(templateBytes);
        using var outputStream = new MemoryStream();

        // Open the existing ODT (zip) and create a new zip to write modified entries
        using (var inputArchive = new ZipArchive(inputStream, ZipArchiveMode.Read, leaveOpen: true))
        using (var outputArchive = new ZipArchive(outputStream, ZipArchiveMode.Create, leaveOpen: true))
        {
            // Copy all entries by default, but for content.xml and styles.xml we will replace text
            foreach (var entry in inputArchive.Entries)
            {
                ZipArchiveEntry newEntry = outputArchive.CreateEntry(entry.FullName, CompressionLevel.Optimal);

                // We'll handle content.xml and styles.xml specially
                if (string.Equals(entry.FullName, "content.xml", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(entry.FullName, "styles.xml", StringComparison.OrdinalIgnoreCase))
                {
                    using var entryStream = entry.Open();
                    using var reader = new StreamReader(entryStream, Encoding.UTF8);
                    string xmlText = reader.ReadToEnd();

                    // Perform replacements
                    xmlText = ReplacePlaceholders(xmlText, alumno);

                    // Write replaced XML into new archive entry
                    using var newEntryStream = newEntry.Open();
                    using var writer = new StreamWriter(newEntryStream, Encoding.UTF8);
                    writer.Write(xmlText);
                    writer.Flush();
                }
                else
                {
                    // Copy other entries verbatim (manifest, meta, pictures, etc.)
                    using var entryStream = entry.Open();
                    using var newEntryStream = newEntry.Open();
                    entryStream.CopyTo(newEntryStream);
                }
            }
        }

        // Return the newly created ODT bytes
        outputStream.Position = 0;
        return outputStream.ToArray();
    }

    public static byte[]? Generate(string type,Alumno alumno)
    {
        if (type == "pdf")
            return GeneratePDF(alumno);
        if (type == "odt")
            return GenerateOdt(alumno);
        if (type == "docx")
            return GenerateDocx(alumno);
        return null;
    }
    private static string ReplacePlaceholders(string xmlText, Alumno alumno)
    {
        // Keep replacements simple - use invariant culture formatting where appropriate
        xmlText = xmlText.Replace("{{fecha}}", DateTime.Now.ToLongDateString(), StringComparison.Ordinal);
        xmlText = xmlText.Replace("{{alumno.nombre}}", alumno.Nombre, StringComparison.Ordinal);
        xmlText = xmlText.Replace("{{alumno.apellido}}", alumno.Apellido, StringComparison.Ordinal);
        xmlText = xmlText.Replace("{{alumno.tipo_documento.sigla}}", alumno.TipoDocumento.ToString(), StringComparison.Ordinal);
        xmlText = xmlText.Replace("{{alumno.nrodocumento}}", alumno.NroDocumento, StringComparison.Ordinal);
        xmlText = xmlText.Replace("{{alumno.nro_legajo}}", alumno.NroLegajo.ToString(), StringComparison.Ordinal);
        xmlText = xmlText.Replace("{{especialidad.nombre}}", alumno.Especialidad.Nombre, StringComparison.Ordinal);
        xmlText = xmlText.Replace("{{facultad.nombre}}", alumno.Especialidad.Facultad.Nombre, StringComparison.Ordinal);
        xmlText = xmlText.Replace("{{universidad.nombre}}", alumno.Especialidad.Facultad.Universidad.Nombre, StringComparison.Ordinal);
        xmlText = xmlText.Replace("{{facultad.ciudad}}", alumno.Especialidad.Facultad.Ciudad, StringComparison.Ordinal);

        return xmlText;
    }
}