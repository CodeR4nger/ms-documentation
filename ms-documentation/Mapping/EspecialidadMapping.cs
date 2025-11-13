using System.Text.Json.Serialization;
using ms_documentation.Models;

namespace ms_documentation.Mapping;

public class EspecialidadDTO
{
    [JsonPropertyName("id")]
    public required int Id { get; set;}
    [JsonPropertyName("especialidad")]
    public required string Nombre { get; set; }
    [JsonPropertyName("facultad")]
    public required string NombreFacultad { get; set; }
    [JsonPropertyName("universidad")]
    public required string NombreUniversidad { get; set; }
}


public static class EspecialidadMapper
{
    public static Especialidad FromDTO(EspecialidadDTO especialidadDTO)
    {
        return new()
        {
            Nombre = especialidadDTO.Nombre,
            Facultad = new()
            {
                Nombre = especialidadDTO.NombreFacultad,
                Universidad = new()
                {
                    Nombre = especialidadDTO.NombreUniversidad
                }
            }
        };
    }
}