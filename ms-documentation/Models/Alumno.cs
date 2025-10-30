namespace ms_documentation.Models;

public class Alumno {
    public required string Apellido { get; set;}
    public required string Nombre { get; set;}
    public required string NroDocumento { get; set; }
    public required TipoDocumento TipoDocumento { get; set;}
    public required int NroLegajo { get; set; }
    public required Especialidad Especialidad { get; set; }
}