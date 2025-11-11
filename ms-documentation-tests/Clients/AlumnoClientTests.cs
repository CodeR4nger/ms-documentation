using System.Net;
using System.Net.Http.Json;
using ms_documentation.Clients;
using ms_documentation.Mapping;
using ms_documentation.Utils;

namespace ms_documentation_tests.Clients;

public class AlumnoClientTests
{
    private readonly AlumnosClient _client;
    private readonly HttpClient _httpClient;
    private readonly EnvironmentHandler _env;

    public AlumnoClientTests()
    {
        _env = new EnvironmentHandler();
        _env.Load();
        _httpClient = new HttpClient { BaseAddress = new Uri(_env.Get("ALUMNOS_API_URI")) };
        _client = new AlumnosClient(_httpClient);
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
        var alumnoDTO = await _client.GetAlumnoByIdAsync(referenceDTO.Id);
        Assert.NotNull(alumnoDTO);

        Assert.Equivalent(referenceDTO, alumnoDTO);
    }
    [Fact]
    public async Task CanHandleInvalidAlumnoId()
    {
        
    }
}