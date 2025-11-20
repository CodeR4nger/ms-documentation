using System.Net;
using System.Net.Http.Headers;
using ms_documentation.Mapping;
using ms_documentation.Utils;

namespace ms_documentation.Clients;

public interface IClienteAlumnos
{
    Task<AlumnoDTO?> GetAlumnoByIdAsync(int id);
}


public class AlumnosClient : IClienteAlumnos
{
    private readonly HttpClient _client;
    private readonly IEnvironmentHandler _env;
    public AlumnosClient(HttpClient client,IEnvironmentHandler env)
    {
        _client = client;
        _env = env;
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }
    public async Task<AlumnoDTO?> GetAlumnoByIdAsync(int id)
    {
        var response = await _client.GetAsync($"{_env.Get("ALUMNOS_API_GET")}/{id}");
        if (response.StatusCode.Equals(HttpStatusCode.NotFound))
            return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AlumnoDTO>();
    } 
}