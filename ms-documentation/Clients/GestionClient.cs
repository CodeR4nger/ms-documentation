using System.Net.Http.Headers;
using ms_documentation.Mapping;
using ms_documentation.Utils;

namespace ms_documentation.Clients;

public interface IClienteGestion
{
    Task<EspecialidadDTO?> GetEspecialidadByIdAsync(int id);
}


public class GestionClient : IClienteGestion
{
    readonly HttpClient _client;
    public GestionClient(HttpClient client)
    {
        _client = client;
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }
    public async Task<EspecialidadDTO?> GetEspecialidadByIdAsync(int id)
    {
        var response = await _client.GetAsync($"{new EnvironmentHandler().Get("GESTION_API_GET")}/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<EspecialidadDTO>();
    } 
}