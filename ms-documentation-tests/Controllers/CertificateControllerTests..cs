
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ms_documentation.Clients;
using ms_documentation.Mapping;
using ms_documentation.Models;
using ms_documentation.Services;
using ms_documentation.Utils;
using ms_documentation_tests.Utils;

namespace ms_documentation_tests.Controllers;

public class CertificateControllerTests : IClassFixture<WebApplicationFactory<ms_documentation.Program>>
{
    private readonly HttpClient _client;
    private readonly IClienteAlumnos _alumnoClient;
    private readonly IClienteGestion _gestionClient;
    private readonly HttpClient _httpClient;
    private readonly EnvironmentHandler _env;
    private readonly AlumnoService _service;

    public CertificateControllerTests(WebApplicationFactory<ms_documentation.Program> factory)
    {
        _client = factory.CreateClient();
        _env = new EnvironmentHandler();
        _env.Load();
        _httpClient = new HttpClient { BaseAddress = new Uri(_env.Get("ALUMNOS_API_URI")) };
        _alumnoClient = new AlumnosClient(new HttpClient { BaseAddress = new Uri(_env.Get("ALUMNOS_API_URI")) },_env);
        _gestionClient = new GestionClient(new HttpClient { BaseAddress = new Uri(_env.Get("GESTION_API_URI")) },_env,null);
        _service = new AlumnoService(_alumnoClient, _gestionClient);
    }
    private async Task<int> GetReferenceAlumnoId()
    {
        var referenceResponse = await _httpClient.GetAsync($"/{_env.Get("ALUMNOS_TEST_API_GET_ALL")}");
        Assert.Equal(HttpStatusCode.OK, referenceResponse.StatusCode);
        List<AlumnoDTO>? alumnoDTOs = await referenceResponse.Content.ReadFromJsonAsync<List<AlumnoDTO>>();
        Assert.NotNull(alumnoDTOs);
        Assert.NotEmpty(alumnoDTOs);
        var referenceId = alumnoDTOs.First().Id;
        return referenceId;
    }

    private async Task<byte[]> RequestCertificate(string requestType, int id)
    {
        var response = await _client.GetAsync($"/api/v1/certificate/{requestType}/{id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Content.Headers.ContentType);
        Assert.Equal($"application/{requestType}", response.Content.Headers.ContentType.MediaType);

        var content = await response.Content.ReadAsByteArrayAsync();
        Assert.NotNull(content);
        Assert.True(content.Length > 0);

        return content;
    }
    [Fact]
    public async Task DoesGeneratePDFCertificate()
    {
        var id = await GetReferenceAlumnoId();
        var referenceAlumno = await _service.GetAlumnoFromId(id);
        Assert.NotNull(referenceAlumno);
        var content = await RequestCertificate("pdf",id);
        var text = CertificateReader.GetPDFText(content);
        CertificateReader.AssertCertificateText(text,referenceAlumno);
    }
    [Fact]
    public async Task DoesGenerateOdtCertificate()
    {
        var id = await GetReferenceAlumnoId();
        var referenceAlumno = await _service.GetAlumnoFromId(id);
        Assert.NotNull(referenceAlumno);
        var content = await RequestCertificate("odt", id);
        var text = CertificateReader.GetOdtText(content);
        CertificateReader.AssertCertificateText(text, referenceAlumno);
    }
    [Fact]
    public async Task DoesGenerateDocxCertificate()
    {
        var id = await GetReferenceAlumnoId();
        var referenceAlumno = await _service.GetAlumnoFromId(id);
        Assert.NotNull(referenceAlumno);
        var content = await RequestCertificate("docx",id);
        var text = CertificateReader.GetDocxText(content);
        CertificateReader.AssertCertificateText(text,referenceAlumno);
    }
}
