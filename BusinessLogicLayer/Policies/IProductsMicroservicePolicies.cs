using Polly;

namespace eCommerce.OrdersService.BusinessLogicLayer.Policies;

public interface IProductsMicroservicePolicies
{
    IAsyncPolicy<HttpResponseMessage> GetFallbackPolicy();

}
