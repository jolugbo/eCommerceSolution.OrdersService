using AutoMapper;
using eCommerce.OrdersService.BusinessLogicLayer.DTO;
using eCommerce.OrdersService.DataAccessLayer.Entities;

namespace eCommerce.OrdersService.BusinessLogicLayer.Mapper
{
    internal class OrderUpdateRequestToOrderMapper: Profile
    {
        public OrderUpdateRequestToOrderMapper()
        {
            CreateMap<OrderUpdateRequest, Order>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserID))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.TotalBill, opt => opt.Ignore())
                .ForMember(dest => dest._id, opt => opt.Ignore());
        }
    }
}
