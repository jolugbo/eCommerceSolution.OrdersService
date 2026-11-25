using Polly;

namespace eCommerce.OrdersService.BusinessLogicLayer.Policies;

public interface IUsersMicroservicePolicies
{
    IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy();

}
