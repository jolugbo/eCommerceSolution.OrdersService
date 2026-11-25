using BusinessLogicLayer.DTO;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Bulkhead;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Retry;
using System.Text;
using System.Text.Json;
namespace eCommerce.OrdersService.BusinessLogicLayer.Policies;

public class ProductsMicroservicePolicies : IProductsMicroservicePolicies
{
    private readonly ILogger<ProductsMicroservicePolicies> _logger;
    public ProductsMicroservicePolicies(ILogger<ProductsMicroservicePolicies> logger)
    {
        _logger = logger;

    }

    public IAsyncPolicy<HttpResponseMessage> GetBulkHeadIsolationPolicy()
    {
        AsyncBulkheadPolicy<HttpResponseMessage> policy = Policy.BulkheadAsync<HttpResponseMessage>(
            maxParallelization:2,//allows 2 concurrent request
            maxQueuingActions:40,//Queue up to 40 additional requests
            onBulkheadRejectedAsync: (context) => {
                _logger.LogWarning("BulkheadIsolation triggered. can't send any more requests since the queue is full");
                throw new BulkheadRejectedException("Bulhead queue is full");
            }
            );
        return policy;
    }

    public IAsyncPolicy<HttpResponseMessage> GetFallbackPolicy()
    {
        AsyncFallbackPolicy<HttpResponseMessage> policy =  
            Policy.HandleResult<HttpResponseMessage>(response => !response.IsSuccessStatusCode)
     .FallbackAsync(async(context) => {
         _logger.LogWarning($"Fallback triggered the request failed, returning dummy data");
         ProductDTO product = new ProductDTO(
             ProductID: Guid.Empty,
             ProductName: "Temporarily Unavailable(fallback)",
             Category: "Temporarily Unavailable(fallback)",
             UnitPrice:0,
             QuantityInStock:0
             );
         return new HttpResponseMessage(System.Net.HttpStatusCode.OK) {
             Content= new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json")
         };

     });
        return policy;
    }
}
