using ms_documentation.Mapping;
using ms_documentation.Models;
using ms_documentation_tests.Utils;

namespace ms_documentation_tests.Mapping;

public class EspecialidadMappingTests
{
    [Fact]
    public void CanConvertEspecialidadDTO()
    {
        Especialidad especialidad = MockDataFactory.CreateEspecialidad();
        EspecialidadDTO referenceDTO = new()
        {
            Id = 1,
            Nombre = especialidad.Nombre,
            NombreFacultad = especialidad.Facultad.Nombre,
            NombreUniversidad = especialidad.Facultad.Universidad.Nombre
        };
        Especialidad especialidadDTO = EspecialidadMapper.FromDTO(referenceDTO);
        Assert.NotNull(especialidadDTO);
        Assert.Equivalent(especialidad, especialidadDTO);
    }
    [Fact]
    public void CanHandleInvalidEspecialidad()
    {
        
    }
}
