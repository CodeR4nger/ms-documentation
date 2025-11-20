using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ms_documentation.Clients;
using ms_documentation.Mapping;
using ms_documentation.Utils;
using StackExchange.Redis;

namespace ms_documentation_tests.Clients;

public class GestionClientTests
{
    private readonly GestionClient _client;
    private readonly HttpClient _httpClient;
    private readonly EnvironmentHandler _env;
    private readonly IDatabase _cache;

    public GestionClientTests()
    {
        _env = new EnvironmentHandler();
        _env.Load();
        var config = new ConfigurationOptions
        {
            EndPoints = { _env.Get("CACHE_ADDRESS") },
            Password = _env.Get("CACHE_PASSWORD"),
            Ssl = false,
            AbortOnConnectFail = false
        };
        _httpClient = new HttpClient { BaseAddress = new Uri(_env.Get("GESTION_API_URI")) };
        _cache = ConnectionMultiplexer.Connect(config).GetDatabase();
        _client = new GestionClient(_httpClient,_env,_cache);
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
    [Fact]
    public async Task CanGetEspecialidadFromCache()
    {
        var getQuery = _env.Get("GESTION_TEST_API_GET_ALL");
        var response = await _httpClient.GetAsync($"/{getQuery}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        List<EspecialidadDTO>? especialidadDTOs = await response.Content.ReadFromJsonAsync<List<EspecialidadDTO>>();
        Assert.NotNull(especialidadDTOs);
        Assert.NotEmpty(especialidadDTOs);
        var referenceDTO = especialidadDTOs.Last();
        Assert.NotNull(referenceDTO);
        referenceDTO.Nombre = "JJHH";
        referenceDTO.Id = -1111;
        await _cache.StringSetAsync(
            $"{_env.Get("GESTION_CACHE_PREFIX")}{referenceDTO.Id}",
            JsonSerializer.Serialize(referenceDTO),
            TimeSpan.FromSeconds(5)
        );
        var especialidadDTO = await _client.GetEspecialidadByIdAsync(referenceDTO.Id);
        Assert.NotNull(especialidadDTO);
        Assert.Equivalent(referenceDTO, especialidadDTO);
    }
}