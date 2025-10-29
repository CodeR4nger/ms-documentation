namespace ms_documentation.Models;

public class Especialidad {
    public int Id { get; set; }
    public required string Nombre { get; set; }
    
    public required Facultad Facultad { get; set; }
}