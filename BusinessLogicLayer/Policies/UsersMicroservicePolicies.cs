using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
namespace eCommerce.OrdersService.BusinessLogicLayer.Policies;

public class UsersMicroservicePolicies :  IUsersMicroservicePolicies
{
    private readonly ILogger<UsersMicroservicePolicies> _logger;
    private readonly IPollyPolicies _pollyPolicies;
    public UsersMicroservicePolicies(ILogger<UsersMicroservicePolicies> logger, IPollyPolicies pollyPolicies)
    {
        _logger = logger;
        _pollyPolicies = pollyPolicies;
    }
    //public IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount) { 
    //    AsyncRetryPolicy<HttpResponseMessage> policy =  Policy.HandleResult<HttpResponseMessage>(response => !response.IsSuccessStatusCode)
    //.WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
    //(outcome, timespan, retryAttempt, context) =>
    //{
    //    _logger.LogInformation($"Retrying request...{retryAttempt} after {timespan.TotalSeconds} seconds");
    //});
    //    return policy;
    //}

    //public IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(int handledEventsAllowedBeforeBreaking, TimeSpan durationOfBreak)
    //{
    //    AsyncCircuitBreakerPolicy<HttpResponseMessage> policy = Policy.HandleResult<HttpResponseMessage>(response => !response.IsSuccessStatusCode)
    // .CircuitBreakerAsync(
    //        handledEventsAllowedBeforeBreaking,durationOfBreak:  durationOfBreak,
    // onBreak:(outcome, timespan) =>
    // {
    //     _logger.LogInformation($"Circuit breaker opened for {timespan.TotalMinutes} minutes. due to consecutive 3 failures. the subsequent requests will be blocked");
    // },onReset: () =>
    // {
    //     _logger.LogInformation("Circuit breaker reset. allowing requests to pass through.");
    // });
    //    return policy;
    //}

    //public IAsyncPolicy<HttpResponseMessage> GetTimeOutPolicy()
    //{
    //    AsyncTimeoutPolicy<HttpResponseMessage> policy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromMilliseconds(1500));
    //    return policy;
    //}
    public IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy()
    {
       var retryPolicy = _pollyPolicies.GetRetryPolicy(3);
       var circuitBreakerPolicy = _pollyPolicies.GetCircuitBreakerPolicy(3, TimeSpan.FromMinutes(2));
       var timeoutPolicy = _pollyPolicies.GetTimeOutPolicy(TimeSpan.FromMilliseconds(1500));
       return retryPolicy.WrapAsync(circuitBreakerPolicy).WrapAsync(timeoutPolicy);
    }
}
