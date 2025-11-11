using System.Text.Json.Serialization;
using ms_documentation.Models;

namespace ms_documentation.Mapping;

public class EspecialidadDTO
{
    [JsonPropertyName("id")]
    public required int Id { get; set;}
    [JsonPropertyName("nombre")]
    public required string Nombre { get; set; }
    [JsonPropertyName("nombre_facultad")]
    public required string NombreFacultad { get; set; }
    [JsonPropertyName("ciudad_facultad")]
    public required string CiudadFacultad { get; set; }
    [JsonPropertyName("nombre_universidad")]
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
                Ciudad = especialidadDTO.CiudadFacultad,
                Universidad = new()
                {
                    Nombre = especialidadDTO.NombreUniversidad
                }
            }
        };
    }
}