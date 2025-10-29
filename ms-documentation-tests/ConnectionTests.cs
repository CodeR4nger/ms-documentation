using Microsoft.AspNetCore.Mvc.Testing;

namespace ms_documentation_tests;

public class ConnectionTests : IClassFixture<WebApplicationFactory<ms_documentation.Program>>
{
    private readonly HttpClient _client;

    public ConnectionTests(WebApplicationFactory<ms_documentation.Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task DoesServiceRespond()
    {
        var response = await _client.GetAsync("/health");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal("API OK", content);
    }
}
