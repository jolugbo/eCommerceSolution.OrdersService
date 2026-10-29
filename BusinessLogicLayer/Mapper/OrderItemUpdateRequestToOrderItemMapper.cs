using eCommerce.OrdersService.BusinessLogicLayer.DTO;
using eCommerce.OrdersService.DataAccessLayer.Entities;
using AutoMapper;
namespace eCommerce.OrdersService.BusinessLogicLayer.Mapper;

public class OrderItemUpdateRequestToOrderItemMapper: Profile
{
    public OrderItemUpdateRequestToOrderItemMapper()
    {
        CreateMap<OrderItemUpdateRequest, OrderItem>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductID))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.TotalPrice, opt => opt.Ignore())
            .ForMember(dest => dest._id, opt => opt.Ignore());
    }
}
