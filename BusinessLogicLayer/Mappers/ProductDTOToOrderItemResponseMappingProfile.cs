using AutoMapper;
using BusinessLogicLayer.DTO;
using eCommerce.OrdersService.BusinessLogicLayer.DTO;
using eCommerce.OrdersService.DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace eCommerce.OrdersService.BusinessLogicLayer.Mappers;

public class ProductDTOToOrderItemResponseMappingProfile : Profile
{
    public ProductDTOToOrderItemResponseMappingProfile()
    {
        CreateMap<ProductDTO, OrderItemResponse>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));
    }
}
