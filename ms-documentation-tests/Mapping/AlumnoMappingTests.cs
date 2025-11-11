using ms_documentation.Mapping;
using ms_documentation.Models;
using ms_documentation_tests.Utils;

namespace ms_documentation_tests.Mapping;

public class AlumnoMappingTests
{
    [Fact]
    public void CanConvertAlumnoDTO()
    {
        Alumno alumno = MockDataFactory.CreateAlumno();
        AlumnoDTO referenceDTO = new()
        {
            Id = 1,
            Apellido = alumno.Apellido,
            Nombre = alumno.Nombre,
            NroDocumento = long.Parse(alumno.NroDocumento),
            TipoDocumento = alumno.TipoDocumento.ToString(),
            NroLegajo = alumno.NroLegajo,
            FechaNacimiento = "11-04-1999",
            FechaIngreso = "12-05-2021",
            Sexo = "F",
            EspecialidadId = 34,
        };
        Alumno alumnoDTO = AlumnoMapper.FromDTO(referenceDTO, alumno.Especialidad);
        Assert.NotNull(alumnoDTO);
        Assert.Equal(alumno.Nombre,alumnoDTO.Nombre);
        Assert.Equal(alumno.Apellido, alumnoDTO.Apellido);
        Assert.Equal(alumno.NroDocumento, alumnoDTO.NroDocumento);
        Assert.Equal(alumno.NroLegajo, alumnoDTO.NroLegajo);
        Assert.Equal(alumno.TipoDocumento, alumnoDTO.TipoDocumento);
        Assert.Equal(alumno.Especialidad, alumnoDTO.Especialidad);
    }
    [Fact]
    public void CanHandleInvalidAlumno()
    {
        
    }
}
