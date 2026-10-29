using AutoMapper;
using eCommerce.OrdersService.BusinessLogicLayer.DTO;
using eCommerce.OrdersService.DataAccessLayer.Entities;

namespace eCommerce.OrdersService.BusinessLogicLayer.Mapper;

internal class OrderItemAddRequestToOrderItemMapper: Profile
{
    public OrderItemAddRequestToOrderItemMapper()
    {
        CreateMap<OrderItemAddRequest, OrderItem>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductID))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.TotalPrice, opt => opt.Ignore());
    }
}
