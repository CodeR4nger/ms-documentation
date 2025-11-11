using System.Text.Json.Serialization;
using ms_documentation.Models;

namespace ms_documentation.Mapping;

public class AlumnoDTO
{
    [JsonPropertyName("id")]
    public required int Id { get; set;}
    [JsonPropertyName("apellido")]
    public required string Apellido { get; set; }
    [JsonPropertyName("nombre")]
    public required string Nombre { get; set; }
    [JsonPropertyName("nro_documento")]
    public required long NroDocumento { get; set; }
    [JsonPropertyName("tipo_documento")]
    public required string TipoDocumento { get; set; }
    [JsonPropertyName("fecha_nacimiento")]
    public required string FechaNacimiento { get; set; }
    [JsonPropertyName("fecha_ingreso")]
    public required string FechaIngreso { get; set; }
    [JsonPropertyName("nro_legajo")]
    public required int NroLegajo { get; set; }
    [JsonPropertyName("sexo")]
    public required string Sexo { get; set; }
    [JsonPropertyName("especialidad_id")]
    public required int EspecialidadId { get; set; }
}


public static class AlumnoMapper
{
    public static Alumno FromDTO(AlumnoDTO alumnoDTO,Especialidad especialidad)
    {
        return new()
        {
            Nombre = alumnoDTO.Nombre,
            Apellido = alumnoDTO.Apellido,
            NroDocumento = alumnoDTO.NroDocumento.ToString(),
            TipoDocumento = Enum.TryParse(alumnoDTO.TipoDocumento, ignoreCase: true, out TipoDocumento tipoDocumento)
                            ? tipoDocumento
                            : throw new ArgumentException($"Tipo de documento inválido: {alumnoDTO.TipoDocumento}"),
            NroLegajo = alumnoDTO.NroLegajo,
            Especialidad = especialidad,
        };
    }
}