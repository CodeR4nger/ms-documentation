using Polly;
using Polly.Timeout;

namespace ms_documentation.Policies;

public static class PollyPolicies
{
    public static IAsyncPolicy<HttpResponseMessage> GetResiliencePolicy()
    {
        // Retry policy: se aplicará en cada intento en caso de timeout o error HTTP.
        var retryPolicy = Policy
            .Handle<TimeoutRejectedException>()  // Maneja TimeoutRejectedException lanzada por Polly
            .Or<OperationCanceledException>()   // Maneja TaskCanceledException lanzada por HttpClient
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)  // Maneja respuestas HTTP con error (por ejemplo, 5xx o 4xx)
            .WaitAndRetryAsync(
                retryCount: 3,  // Máximo de 3 reintentos
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),  // Estrategia de reintentos exponencial
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    // Esto es opcional: se ejecuta cada vez que ocurre un reintento
                    Console.WriteLine($"Retry {retryCount} after {timeSpan.TotalSeconds} seconds due to {exception.Exception?.Message ?? exception.Result.StatusCode.ToString()}");
                }
            );

        // Timeout policy: se aplica por cada intento individual.
        var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(3));  // Timeout de 3 segundos para cada intento.

        // Combinar el RetryPolicy con el TimeoutPolicy, pero sin intentar manejar TaskCanceledException en la envoltura
        var combinedPolicy = retryPolicy.WrapAsync(timeoutPolicy);

        // Manejar explícitamente TaskCanceledException, transformándola en TimeoutRejectedException
        return Policy
            .Handle<TaskCanceledException>()
            .RetryAsync(3, onRetry: (exception, retryCount) =>
            {
                // Lanzar TimeoutRejectedException si se detecta un TaskCanceledException
                throw new TimeoutRejectedException("The request timed out after retries.", exception);
            })
            .WrapAsync(combinedPolicy);
    }

}