using Microsoft.AspNetCore.Mvc.Testing;

namespace ms_documentation_tests.Controllers;

public class ConnectionTests(WebApplicationFactory<ms_documentation.Program> factory) 
                : IClassFixture<WebApplicationFactory<ms_documentation.Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task DoesServiceRespond()
    {
        var response = await _client.GetAsync("/");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal("API OK", content);
    }
}
