using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace eCommerce.OrdersService.DataAccessLayer.Entities;

public class Order
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public Guid _id { get; set; }
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public Guid OrderId { get; set; }

    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public Guid UserId { get; set; }
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public DateTime OrderDate { get; set; }
    [BsonRepresentation(MongoDB.Bson.BsonType.Double)]
    public decimal TotalBill { get; set; }
    public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
