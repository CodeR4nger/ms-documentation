using System.Net;
using System.Net.Http.Json;
using ms_documentation.Clients;
using ms_documentation.Mapping;
using ms_documentation.Services;
using ms_documentation.Utils;

namespace ms_documentation_tests.Services;

public class AlumnoServiceTests
{
    private readonly IClienteAlumnos _alumnoClient;
    private readonly IClienteGestion _gestionClient;
    private readonly EnvironmentHandler _env;
    private readonly HttpClient _httpClient;
    private readonly AlumnoService _service;
    public AlumnoServiceTests()
    {
        _env = new EnvironmentHandler();
        _env.Load();
        _httpClient = new HttpClient { BaseAddress = new Uri(_env.Get("ALUMNOS_API_URI")) };
        _alumnoClient = new AlumnosClient(_httpClient);
        _gestionClient = new GestionClient(new HttpClient { BaseAddress = new Uri(_env.Get("GESTION_API_URI")) });
        _service = new AlumnoService(_alumnoClient, _gestionClient);
    }

    [Fact]
    public async Task CanGetAlumnoFromId()
    {
        var getQuery = _env.Get("ALUMNOS_TEST_API_GET_ALL");
        var response = await _httpClient.GetAsync($"/{getQuery}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        List<AlumnoDTO>? alumnoDTOs = await response.Content.ReadFromJsonAsync<List<AlumnoDTO>>();
        Assert.NotNull(alumnoDTOs);
        Assert.NotEmpty(alumnoDTOs);
        var referenceDTO = alumnoDTOs.First();
        Assert.NotNull(referenceDTO);
        var alumnoDTO = await _alumnoClient.GetAlumnoByIdAsync(referenceDTO.Id);
        Assert.NotNull(alumnoDTO);
        var especialidadDTO = await _gestionClient.GetEspecialidadByIdAsync(referenceDTO.EspecialidadId);
        Assert.NotNull(especialidadDTO);
        var referenceAlumno = AlumnoMapper.FromDTO(alumnoDTO, EspecialidadMapper.FromDTO(especialidadDTO));
        Assert.NotNull(referenceAlumno);
        var alumno = await _service.GetAlumnoFromId(referenceDTO.Id);
        Assert.NotNull(alumno);

        Assert.Equivalent(referenceAlumno, alumno);
    }
}
