using System.Threading.Tasks;
using ms_documentation.Clients;
using ms_documentation.Mapping;
using ms_documentation.Models;

namespace ms_documentation.Services;

public class AlumnoService(IClienteAlumnos clienteAlumnos, IClienteGestion clienteGestion)
{
    private readonly IClienteAlumnos _alumnoClient = clienteAlumnos;
    private readonly IClienteGestion _gestionClient = clienteGestion;

    public async Task<Alumno?> GetAlumnoFromId(int id)
    {
        AlumnoDTO? rawAlumno = await _alumnoClient.GetAlumnoByIdAsync(id);

        if (rawAlumno == null)
            return null;

        EspecialidadDTO? rawEspecialidad = await _gestionClient.GetEspecialidadByIdAsync(rawAlumno.EspecialidadId);

        if (rawEspecialidad == null)
            return null;

        Especialidad especialidad = EspecialidadMapper.FromDTO(rawEspecialidad);

        return AlumnoMapper.FromDTO(rawAlumno, especialidad);

    }
}