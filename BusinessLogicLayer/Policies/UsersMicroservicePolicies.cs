using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
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

}
