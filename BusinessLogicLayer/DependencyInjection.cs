using eCommerce.OrdersService.BusinessLogicLayer.Mapper;
using eCommerce.OrdersService.BusinessLogicLayer.Validator;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.OrdersService.BusinessLogicLayer;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLogicLayer(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddValidatorsFromAssemblyContaining<OrderAddRequestValidator>();
        services.AddAutoMapper(typeof(OrderItemToOrderOrderItemResponseMapper).Assembly);

        return services;
    }
}
