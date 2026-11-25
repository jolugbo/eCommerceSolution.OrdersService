using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
namespace eCommerce.OrdersService.BusinessLogicLayer.Policies;

public class UsersMicroservicePolicies : IUsersMicroservicePolicies
{
    private readonly ILogger<UsersMicroservicePolicies> _logger;
    public UsersMicroservicePolicies(ILogger<UsersMicroservicePolicies> logger)
    {
        _logger = logger;

    }
    public IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() { 
        AsyncRetryPolicy<HttpResponseMessage> policy =  Policy.HandleResult<HttpResponseMessage>(response => !response.IsSuccessStatusCode)
    .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
    (outcome, timespan, retryAttempt, context) =>
    {
        _logger.LogInformation($"Retrying request...{retryAttempt} after {timespan.TotalSeconds} seconds");
    });
        return policy;
    }

    public IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        AsyncCircuitBreakerPolicy<HttpResponseMessage> policy = Policy.HandleResult<HttpResponseMessage>(response => !response.IsSuccessStatusCode)
     .CircuitBreakerAsync(
            3,durationOfBreak:  TimeSpan.FromMinutes(2),
     onBreak:(outcome, timespan) =>
     {
         _logger.LogInformation($"Circuit breaker opened for {timespan.TotalMinutes} minutes. due to consecutive 3 failures. the subsequent requests will be blocked");
     },onReset: () =>
     {
         _logger.LogInformation("Circuit breaker reset. allowing requests to pass through.");
     });
        return policy;
    }

    public IAsyncPolicy<HttpResponseMessage> GetTimeOutPolicy()
    {
        AsyncTimeoutPolicy<HttpResponseMessage> policy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromMilliseconds(1500));
        return policy;
    }
}
