using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
namespace eCommerce.OrdersService.BusinessLogicLayer.Policies;

public class PollyPolicies : IPollyPolicies
{
    private readonly ILogger<UsersMicroservicePolicies> _logger;
    public PollyPolicies(ILogger<UsersMicroservicePolicies> logger)
    {
        _logger = logger;

    }
    public IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount) { 
        AsyncRetryPolicy<HttpResponseMessage> policy =  Policy.HandleResult<HttpResponseMessage>(response => !response.IsSuccessStatusCode)
    .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
    (outcome, timespan, retryAttempt, context) =>
    {
        _logger.LogInformation($"Retrying request...{retryAttempt} after {timespan.TotalSeconds} seconds");
    });
        return policy;
    }

    public IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(int handledEventsAllowedBeforeBreaking, TimeSpan durationOfBreak)
    {
        AsyncCircuitBreakerPolicy<HttpResponseMessage> policy = Policy.HandleResult<HttpResponseMessage>(response => !response.IsSuccessStatusCode)
     .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking,durationOfBreak:  durationOfBreak,
     onBreak:(outcome, timespan) =>
     {
         _logger.LogInformation($"Circuit breaker opened for {timespan.TotalMinutes} minutes. due to consecutive {handledEventsAllowedBeforeBreaking} failures. the subsequent requests will be blocked");
     },onReset: () =>
     {
         _logger.LogInformation("Circuit breaker reset. allowing requests to pass through.");
     });
        return policy;
    }

    public IAsyncPolicy<HttpResponseMessage> GetTimeOutPolicy(TimeSpan timeout)
    {
        AsyncTimeoutPolicy<HttpResponseMessage> policy = Policy.TimeoutAsync<HttpResponseMessage>(timeout);
        return policy;
    }
}
