using System.Net.Http.Headers;
using ms_documentation.Mapping;
using ms_documentation.Utils;
using StackExchange.Redis;

namespace ms_documentation.Clients;

public interface IClienteGestion
{
    Task<EspecialidadDTO?> GetEspecialidadByIdAsync(int id);
}


public class GestionClient : IClienteGestion
{
    private readonly HttpClient _client;
    private readonly IDatabase? _cache;
    private readonly IEnvironmentHandler _env;

    public GestionClient(HttpClient client,IEnvironmentHandler env, IDatabase? cache = null)
    {
        _client = client;
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        _env = env;
        _cache = cache;
    }

    public async Task<EspecialidadDTO?> GetEspecialidadByIdAsync(int id)
    {
        string cacheKey = $"especialidad_{id}";

        if (_cache != null)
        {
            string? cached = await _cache.StringGetAsync(cacheKey);

            if (!string.IsNullOrEmpty(cached))
            {
                return System.Text.Json.JsonSerializer.Deserialize<EspecialidadDTO>(cached);
            }
        }

        var url = $"{_env.Get("GESTION_API_GET")}/{id}";
        var response = await _client.GetAsync(url);

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<EspecialidadDTO>();
    }
}
