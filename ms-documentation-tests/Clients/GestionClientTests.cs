using System.Net;
using System.Net.Http.Json;
using ms_documentation.Clients;
using ms_documentation.Mapping;
using ms_documentation.Utils;

namespace ms_documentation_tests.Clients;

public class GestionClientTests
{
    private readonly GestionClient _client;
    private readonly HttpClient _httpClient;
    private readonly EnvironmentHandler _env;

    public GestionClientTests()
    {
        _env = new EnvironmentHandler();
        _env.Load();
        _httpClient = new HttpClient { BaseAddress = new Uri(_env.Get("GESTION_API_URI")) };
        _client = new GestionClient(_httpClient,null);
    }

    [Fact]
    public async Task CanGetEspecialidadFromId()
    {
        var getQuery = _env.Get("GESTION_TEST_API_GET_ALL");
        var response = await _httpClient.GetAsync($"/{getQuery}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        List<EspecialidadDTO>? especialidadDTOs = await response.Content.ReadFromJsonAsync<List<EspecialidadDTO>>();
        Assert.NotNull(especialidadDTOs);
        Assert.NotEmpty(especialidadDTOs);
        var referenceDTO = especialidadDTOs.First();
        Assert.NotNull(referenceDTO);
        var especialidadDTO = await _client.GetEspecialidadByIdAsync(referenceDTO.Id);
        Assert.NotNull(especialidadDTO);

        Assert.Equivalent(referenceDTO, especialidadDTO);
    }
    [Fact]
    public async Task CanHandleInvalidEspecialidadId()
    {
        await Assert.ThrowsAsync<HttpRequestException>(async () =>
        {
            var especialidadDTO = await _client.GetEspecialidadByIdAsync(-1);
        });
    }

}