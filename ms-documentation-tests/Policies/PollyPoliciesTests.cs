using Moq;
using Moq.Protected;
using ms_documentation.Policies;
using Polly;
using Polly.Timeout;

namespace ms_documentation_tests.Policies;

public class PollyPoliciesTests
{
    [Fact]
    public async Task Should_Retry_On_Timeout_Exception()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new TaskCanceledException());  // Excepción típica de timeout en HttpClient

        var client = new HttpClient(handlerMock.Object);
        
        var policy = PollyPolicies.GetResiliencePolicy();

        // Act
        var exception = await Assert.ThrowsAsync<TimeoutRejectedException>(async () =>
        {
            await policy.ExecuteAsync(ctx => client.GetAsync("https://example.com"), new Context());
        });

        // Assert
        // Verificamos que la política haya intentado reintentar (con 3 intentos, como la configuración en el retryPolicy)
        handlerMock.Protected().Verify(
            "SendAsync", 
            Times.AtLeast(3),  // Se debería haber intentado al menos 3 veces
            ItExpr.IsAny<HttpRequestMessage>(), 
            ItExpr.IsAny<CancellationToken>());
    }

}
