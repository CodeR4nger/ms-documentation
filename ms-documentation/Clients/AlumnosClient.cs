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
    readonly HttpClient _client;
    public AlumnosClient(HttpClient client)
    {
        _client = client;
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }
    public async Task<AlumnoDTO?> GetAlumnoByIdAsync(int id)
    {
        var response = await _client.GetAsync($"{new EnvironmentHandler().Get("ALUMNOS_API_GET")}/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AlumnoDTO>();
    } 
}