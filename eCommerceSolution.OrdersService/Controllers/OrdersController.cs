using eCommerce.OrdersService.BusinessLogicLayer.DTO;
using eCommerce.OrdersService.BusinessLogicLayer.ServiceContracts;
using eCommerce.OrdersService.DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace eCommerceSolution.OrdersService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService _ordersService;

        public OrdersController(IOrdersService ordersService)
        {
            _ordersService = ordersService;
        }
        //GET api/Orders
        [HttpGet]
        public async Task<IEnumerable<OrderResponse?>> Get()
        {
            List<OrderResponse?> response = await _ordersService.GetOrders();
            
            return response;
        }

        //GET api/Orders/search/orderid/{orderID}
        [HttpGet("search/orderid/{orderID}")]
        public async Task<OrderResponse?> GetOrderByOrderID(Guid orderID)
        {
            FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(temp => temp.OrderID, orderID);
            OrderResponse response = await _ordersService.GetOrderByCondition(filter);

            return response;
        }
        
        //GET api/Orders/search/productid/{productID}
        [HttpGet("search/productid/{productID}")]
        public async Task<IEnumerable<OrderResponse?>> GetOrderByProductID(Guid productID)
        {
            FilterDefinition<Order> filter =
                Builders<Order>.Filter.ElemMatch(temp => temp.OrderItems,
                Builders<OrderItem>.Filter.Eq(tempProduct => tempProduct.ProductID, productID));
            List<OrderResponse?> response = await _ordersService.GetOrdersByCondition(filter);

            return response;
        }

        //GET api/Orders/search/orderDate/{orderDate}
        [HttpGet("search/orderDate/{orderDate}")]
        public async Task<IEnumerable<OrderResponse?>> GetOrderByOrderDate(DateTime orderDate)
        {
            FilterDefinition<Order> filter =
                Builders<Order>.Filter.Eq(temp => temp.OrderDate.ToString("yyyy-MM-dd"), orderDate.ToString("yyyy-MM-dd"));
            List<OrderResponse?> response = await _ordersService.GetOrdersByCondition(filter);

            return response;
        }

        //GET api/Orders/search/userID/{userID}
        [HttpGet("search/userID/{userID}")]
        public async Task<IEnumerable<OrderResponse?>> GetOrderByUserID(Guid userID)
        {
            FilterDefinition<Order> filter =
                Builders<Order>.Filter.Eq(temp => temp.UserID, userID);
            List<OrderResponse?> response = await _ordersService.GetOrdersByCondition(filter);

            return response;
        }

        //POST api/orders
        [HttpPost]
        public async Task<IActionResult> orders(OrderAddRequest orderRequest)
        {
            if (orderRequest == null)
            {
                return BadRequest("Invalid order data");
            }
            OrderResponse? response = await _ordersService.AddOrder(orderRequest);
            if (response == null)
            {
                return Problem("Order creation failed");
            }
            return Created($"api/Orders/search/orderid/{response?.OrderID}", response);
        }

        //PUT api/orders/{orderID}
        [HttpPut("{orderID}")]
        public async Task<IActionResult> UpdateOrder(Guid orderID, OrderUpdateRequest orderUpdateRequest)
        {
            if (orderUpdateRequest == null)
            {
                return BadRequest("Invalid order data");
            }
            if (orderID != orderUpdateRequest.OrderID)
            {
                return BadRequest("Order ID mismatch");
            }
            OrderResponse? response = await _ordersService.UpdateOrder(orderUpdateRequest);
            if (response == null)
            {
                return Problem("Order creation failed");
            }
            return Created($"api/Orders/search/orderid/{response?.OrderID}", response);
        }

        //DELETE api/orders/{orderID}
        [HttpDelete("{orderID}")]
        public async Task<IActionResult> DeleteOrder(Guid orderID)
        {
            if (orderID == Guid.Empty)
            {
                return BadRequest("Invalid order ID");
            }
            bool isDeleted = await _ordersService.DeleteOrder(orderID);
            if (!isDeleted)
            {
                return NotFound("Order not found");
            }
            return Ok("Order deleted successfully");
        }
    }
}
