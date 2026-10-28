using eCommerce.OrdersService.BusinessLogicLayer.DTO;
using eCommerce.OrdersService.BusinessLogicLayer.ServiceContracts;
using eCommerce.OrdersService.DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace eCommerce.OrdersService.usinessLogicLayer.Services
{
    public class OrdersService : IOrdersService
    {
        public Task<OrderResponse?> AddOrder(OrderAddRequest orderRequest)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteOrder(Guid orderID)
        {
            throw new NotImplementedException();
        }

        public Task<OrderResponse?> GetOrderByCondition(Func<Order?> filter)
        {
            throw new NotImplementedException();
        }

        public Task<List<OrderResponse?>> GetOrders()
        {
            throw new NotImplementedException();
        }

        public Task<List<OrderResponse?>> GetOrdersByCondition(Func<Order?> filter)
        {
            throw new NotImplementedException();
        }

        public Task<OrderResponse?> UpdateOrder(OrderUpdateRequest orderUpdateRequest)
        {
            throw new NotImplementedException();
        }
    }
}
