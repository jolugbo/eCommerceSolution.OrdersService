using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace eCommerce.OrdersService.BusinessLogicLayer.DTO;

public record OrderItemResponse(Guid ProductID, string ProductName,string Category, decimal UnitPrice, int Quantity, decimal TotalPrice)
{
    public OrderItemResponse() : this(default, default, default, default, default, default)
    {
    }
}