using AutoMapper;
using eCommerce.OrdersService.BusinessLogicLayer.DTO;
using eCommerce.OrdersService.DataAccessLayer.Entities;

namespace eCommerce.OrdersService.BusinessLogicLayer.Mapper;

public class OrderToOrderResponseMapper: Profile
{
    public OrderToOrderResponseMapper()
    {
        CreateMap<Order, OrderResponse>()
            .ForMember(dest => dest.OrderID, opt => opt.MapFrom(src => src.OrderId))
            .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
            .ForMember(dest => dest.TotalBill, opt => opt.MapFrom(src => src.TotalBill));
    }
}
