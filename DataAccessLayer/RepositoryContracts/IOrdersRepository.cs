using System;
using System.Collections.Generic;
using System.Text;
using eCommerce.OrdersService.DataAccessLayer.Entities;
using MongoDB.Driver;

namespace eCommerce.OrdersService.DataAccessLayer.RepositoryContracts;

public interface IOrdersRepository
{
    /// <summary>
    /// Retrieves a collection of orders asynchronously.    
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of orders.</returns>
    Task<IEnumerable<Order>> GetOrders();
    /// <summary>
    /// Retrieves orders matching the specified filter condition asynchronously.    
    /// </summary>
    /// <param name="filter">A filter definition specifying the criteria for selecting orders.</param>
    /// <returns>A task representing the asynchronous operation, containing a collection of orders that match the filter.</returns>
    Task<IEnumerable<Order?>> GetOrdersByCondition(FilterDefinition<Order> filter);
    /// <summary>
    /// Retrieves an order matching the specified filter condition asynchronously.    
    /// </summary>
    /// <param name="filter">A filter definition specifying the criteria for selecting the order.</param>
    /// <returns>A task representing the asynchronous operation, containing the order that matches the filter, or null if no matching order is found.</returns>
    Task<Order?> GetOrderByCondition(FilterDefinition<Order> filter);
    /// <summary>
    /// Adds a new order asynchronously.    
    /// </summary>
    /// <param name="order">The order to add.</param>
    /// <returns>A task representing the asynchronous operation, containing the added order.</returns>
    Task<Order?> AddOrder(Order order);
    /// <summary>
    /// Updates an existing order asynchronously.    
    /// </summary>
    /// <param name="order">The order to update.</param>
    /// <returns>A task representing the asynchronous operation, containing the updated order.</returns>
    Task<Order?> UpdateOrder(Order order);
    /// <summary>
    /// Deletes an order asynchronously.    
    /// </summary>
    /// <param name="orderId">The ID of the order to delete.</param>
    /// <returns>A task representing the asynchronous operation, indicating whether the deletion was successful.</returns>
    Task<bool> DeleteOrder(Guid orderId);
}
