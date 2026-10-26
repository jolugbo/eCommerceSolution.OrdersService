using eCommerce.OrdersService.DataAccessLayer.Entities;
using eCommerce.OrdersService.DataAccessLayer.RepositoryContracts;
using MongoDB.Driver;
using static System.Net.WebRequestMethods;

namespace eCommerce.OrdersService.DataAccessLayer.Repositories;

public class OrdersRepository : IOrdersRepository
{
    private readonly IMongoCollection<Order> _ordersCollection;

    public OrdersRepository(IMongoDatabase database)
    {
        _ordersCollection = database.GetCollection<Order>("orders");
    }   
    public async Task<IEnumerable<Order>> GetOrders()
    {
        return (await _ordersCollection.FindAsync(Builders<Order>.Filter.Empty)).ToList();
    }

    public async Task<IEnumerable<Order?>> GetOrdersByCondition(FilterDefinition<Order> filter)
    {
        return (await _ordersCollection.FindAsync(filter)).ToList();
    }

    public async Task<Order?> GetOrderByCondition(FilterDefinition<Order> filter)
    {
        return (await _ordersCollection.FindAsync(filter)).FirstOrDefault();
    }

    public async Task<Order?> AddOrder(Order order)
    {
        order.OrderId = Guid.NewGuid();
         await _ordersCollection.InsertOneAsync(order);
        return order;
    }

    public async Task<Order?> UpdateOrder(Order order)
    {
        FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(x => x.OrderId, order.OrderId);
        var existingOrder = (await _ordersCollection.FindAsync(filter)).FirstOrDefault();
        if (existingOrder == null)
        {
            return null;
        }
        ReplaceOneResult updateResult = await _ordersCollection.ReplaceOneAsync(filter,order);
        return order;
    }

    public async Task<bool> DeleteOrder(Guid orderId)
    {
        FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(x => x.OrderId, orderId);
        var existingOrder = (await _ordersCollection.FindAsync(filter)).FirstOrDefault(); 
        if(existingOrder == null)
        {
            return false;
        }
        DeleteResult deleteResult =  await _ordersCollection.DeleteOneAsync(filter); 
        return deleteResult.DeletedCount > 0;
    }
}
