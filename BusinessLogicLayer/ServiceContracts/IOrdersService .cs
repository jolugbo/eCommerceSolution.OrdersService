using eCommerce.OrdersService.BusinessLogicLayer.DTO;
using eCommerce.OrdersService.DataAccessLayer.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace eCommerce.OrdersService.BusinessLogicLayer.ServiceContracts;

public interface IOrdersService
{
    /// <summary>
    ///     
    /// </summary>
    /// <returns>Returns a list of orders</returns>
    Task<List<OrderResponse?>> GetOrders();
    /// <summary>
    /// 
    /// </summary>
    /// <param name="filter"></param>
    /// <returns>Returns a list of orders matching the condition</returns>
    Task<List<OrderResponse>?> GetOrdersByCondition(FilterDefinition<Order> filter);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="filter"></param>
    /// <returns>Returns an order matching the condition</returns>
    Task<OrderResponse?> GetOrderByCondition(FilterDefinition<Order?> filter);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="orderRequest"></param>
    /// <returns>Returns the added order</returns>
    Task<OrderResponse?> AddOrder(OrderAddRequest orderRequest);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="orderUpdateRequest"></param>
    /// <returns>Returns the updated order</returns>
    Task<OrderResponse?> UpdateOrder(OrderUpdateRequest orderUpdateRequest);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="orderID"></param>
    /// <returns>Returns a boolean indicating if the order was deleted</returns>
    Task<bool> DeleteOrder(Guid orderID);
}
