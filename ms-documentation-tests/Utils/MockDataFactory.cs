using ms_documentation.Models;

namespace ms_documentation_tests.Utils;

public static class MockDataFactory
{
    public static Alumno CreateAlumno() => new()
    {
        Apellido = "Gomez",
        Nombre = "Juan",
        NroDocumento = "12345678",
        TipoDocumento = TipoDocumento.DNI,
        NroLegajo = 1001,
        Especialidad = CreateEspecialidad()
    };
    public static Especialidad CreateEspecialidad() => new()
    {
        Nombre = "Ingenieria de Software",
        Facultad = CreateFacultad()
    };
    public static Facultad CreateFacultad() => new()
    {
        Nombre = "Facultad de Sistemas",
        Universidad = CreateUniversidad(),
    };
    public static Universidad CreateUniversidad() => new()
    {
        Nombre = "Universidad Tecnologica Nacional", 
    };
}