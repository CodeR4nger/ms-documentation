using System.Net;
using Polly;
using Polly.Timeout;

namespace ms_documentation.Policies;

public static class PollyPolicies
{
    public static IAsyncPolicy<HttpResponseMessage> GetResiliencePolicy()
    {
        var retryPolicy = Policy
            .Handle<TimeoutRejectedException>()  // Maneja TimeoutRejectedException lanzada por Polly
            .Or<OperationCanceledException>()   // Maneja TaskCanceledException lanzada por HttpClient
            .OrResult<HttpResponseMessage>(r => 
                !r.IsSuccessStatusCode && 
                r.StatusCode != HttpStatusCode.NotFound && 
                r.StatusCode != HttpStatusCode.BadRequest)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    Console.WriteLine($"Retry {retryCount} after {timeSpan.TotalSeconds} seconds due to {exception.Exception?.Message ?? exception.Result.StatusCode.ToString()}");
                }
            );

        var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(3));  // Timeout de 3 segundos para cada intento.

        var combinedPolicy = retryPolicy.WrapAsync(timeoutPolicy);

        return Policy
            .Handle<TaskCanceledException>()
            .RetryAsync(3, onRetry: (exception, retryCount) =>
            {
                throw new TimeoutRejectedException("The request timed out after retries.", exception);
            })
            .WrapAsync(combinedPolicy);
    }

}