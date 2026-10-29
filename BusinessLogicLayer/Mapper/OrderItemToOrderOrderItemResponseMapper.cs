using AutoMapper;
using eCommerce.OrdersService.BusinessLogicLayer.DTO;
using eCommerce.OrdersService.DataAccessLayer.Entities;

namespace eCommerce.OrdersService.BusinessLogicLayer.Mapper;

public class OrderItemToOrderOrderItemResponseMapper : Profile
{
    public OrderItemToOrderOrderItemResponseMapper()
    {
        CreateMap<OrderItem, OrderItemResponse>()
            .ForMember(dest => dest.ProductID, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice));
    }
}
