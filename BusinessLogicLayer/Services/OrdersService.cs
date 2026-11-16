using AutoMapper;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.OrdersService.BusinessLogicLayer.DTO;
using BusinessLogicLayer.OrdersService.BusinessLogicLayer.HTTPClients;
using eCommerce.OrdersService.BusinessLogicLayer.DTO;
using eCommerce.OrdersService.BusinessLogicLayer.ServiceContracts;
using eCommerce.OrdersService.BusinessLogicLayer.Validator;
using eCommerce.OrdersService.DataAccessLayer.Entities;
using eCommerce.OrdersService.DataAccessLayer.RepositoryContracts;
using FluentValidation;
using FluentValidation.Results;
using MongoDB.Driver;

namespace eCommerce.OrdersService.usinessLogicLayer.Services
{
    public class OrderService : IOrdersService
    {
        private readonly IOrdersRepository _ordersRepository;
        private readonly IMapper _mapper;
        private readonly OrderAddRequestValidator _orderAddRequestValidator;
        private readonly IValidator<OrderItemAddRequest> _orderItemAddRequestValidator;
        private readonly IValidator<OrderUpdateRequest> _orderUpdateRequestValidator;
        private readonly IValidator<OrderItemUpdateRequest> _orderItemUpdateRequestValidator;
        private readonly UsersMicroserviceClient _usersMicroserviceClient;
        private readonly ProductsMicroserviceClient _productsMicroserviceClient;

        public OrderService(IOrdersRepository ordersRepository, IMapper mapper, OrderAddRequestValidator orderAddRequestValidator, IValidator<OrderItemAddRequest> orderItemAddRequestValidator,
            IValidator<OrderUpdateRequest> orderUpdateRequestValidator, IValidator<OrderItemUpdateRequest> orderItemUpdateRequestValidator, UsersMicroserviceClient usersMicroserviceClient, ProductsMicroserviceClient productsMicroserviceClient)
        {
            _ordersRepository = ordersRepository;
            _mapper = mapper;
            _orderAddRequestValidator = orderAddRequestValidator;
            _orderItemAddRequestValidator = orderItemAddRequestValidator;
            _orderUpdateRequestValidator = orderUpdateRequestValidator;
            _orderItemUpdateRequestValidator = orderItemUpdateRequestValidator;
            _usersMicroserviceClient = usersMicroserviceClient;
            _productsMicroserviceClient = productsMicroserviceClient;
        }

        public async Task<OrderResponse?> AddOrder(OrderAddRequest orderAddRequest)
        {
            if(orderAddRequest == null)
            {
                throw new ArgumentNullException(nameof(orderAddRequest));
            }
           ValidationResult orderAddRequestValidationResult = await _orderAddRequestValidator.ValidateAsync(orderAddRequest);
           if(!orderAddRequestValidationResult.IsValid)
            {
                string errors = string.Join(", ", orderAddRequestValidationResult.Errors.Select(e => e.ErrorMessage));
                throw new ValidationException(errors);
            }
            List<ProductDTO?> products = new List<ProductDTO?>();
            foreach ( OrderItemAddRequest orderItemAddRequest in orderAddRequest.OrderItems)
            {
                ValidationResult orderItemAddRequestValidationResult = await _orderItemAddRequestValidator.ValidateAsync(orderItemAddRequest);
                if (!orderItemAddRequestValidationResult.IsValid)
                {
                    string errors = string.Join(", ", orderItemAddRequestValidationResult.Errors.Select(e => e.ErrorMessage));
                    throw new ValidationException(errors);
                }
                ProductDTO product = await _productsMicroserviceClient.GetProductById(orderItemAddRequest.ProductID);
                if (product == null)
                {
                    throw new Exception($"Product with ID {orderItemAddRequest.ProductID} not found.");
                }
                products.Add(product);
            }
            UserDTO user = await _usersMicroserviceClient.GetUserById(orderAddRequest.UserID);
            if (user == null)
            {
                throw new Exception($"User with ID {orderAddRequest.UserID} not found.");
            }
            var orderEntity = _mapper.Map<Order>(orderAddRequest);
            orderEntity.UserID = user.UserId;
            foreach (var orderItem in orderEntity.OrderItems)
            {
                orderItem.TotalPrice = orderItem.Quantity * orderItem.UnitPrice;
            }
            orderEntity.TotalBill = orderEntity.OrderItems.Sum(oi => oi.TotalPrice);

            var addedOrderEntity = await _ordersRepository.AddOrder(orderEntity);
            var addedOrderResponse = _mapper.Map<OrderResponse>(addedOrderEntity);
            if (addedOrderResponse != null)
            {
                foreach (OrderItemResponse orderItem in addedOrderResponse.OrderItems)
                {
                    ProductDTO productDTO = products.Where(temp => temp.ProductID == orderItem.ProductID).FirstOrDefault();
                    if (productDTO == null) { continue; }
                    _mapper.Map<ProductDTO, OrderItemResponse>(productDTO, orderItem);

                }
            }
            return addedOrderResponse;
        }

        public async Task<bool> DeleteOrder(Guid orderID)
        {
            FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(temp => temp.OrderID, orderID);
            Order? existingOrder = await _ordersRepository.GetOrderByCondition(filter);
            if (existingOrder == null) return false;
           bool isDeleted = await _ordersRepository.DeleteOrder(orderID);
            return isDeleted;
        }

        public async Task<OrderResponse?> GetOrderByCondition(FilterDefinition<Order?> filter)
        {
            Order? orders = await _ordersRepository.GetOrderByCondition(filter);
            OrderResponse? orderResponse = _mapper.Map<OrderResponse?>(orders);
            if (orderResponse!= null)
            {
                foreach (OrderItemResponse orderItem in orderResponse.OrderItems)
                {
                    ProductDTO productDTO = await _productsMicroserviceClient.GetProductById(orderItem.ProductID);
                    if (productDTO == null) { continue; }
                    _mapper.Map<ProductDTO, OrderItemResponse>(productDTO, orderItem);

                }
            }
           
            return orderResponse;
        }

        public async Task<List<OrderResponse?>> GetOrders()
        {
            IEnumerable<Order>? orders = await _ordersRepository.GetOrders();

            IEnumerable<OrderResponse>? orderResponses = _mapper.Map<List<OrderResponse?>>(orders);
            foreach (OrderResponse orderResponse in orderResponses)
            {
                if (orderResponse == null) {  continue; }
                foreach (OrderItemResponse orderItem in orderResponse.OrderItems)
                {
                    ProductDTO productDTO = await _productsMicroserviceClient.GetProductById(orderItem.ProductID);
                    if (productDTO == null) {  continue; }
                    _mapper.Map<ProductDTO, OrderItemResponse>(productDTO, orderItem);
                    
                }
            }
            return orderResponses.ToList();

        }

        public async Task<List<OrderResponse?>> GetOrdersByCondition(FilterDefinition<Order> filter)
        {
            IEnumerable<Order>? orders = await _ordersRepository.GetOrdersByCondition(filter);
            IEnumerable<OrderResponse>? orderResponses = _mapper.Map<List<OrderResponse?>>(orders);
            foreach (OrderResponse orderResponse in orderResponses)
            {
                if (orderResponse == null) { continue; }
                foreach (OrderItemResponse orderItem in orderResponse.OrderItems)
                {
                    ProductDTO productDTO = await _productsMicroserviceClient.GetProductById(orderItem.ProductID);
                    if (productDTO == null) { continue; }
                    _mapper.Map<ProductDTO, OrderItemResponse>(productDTO, orderItem);

                }
            }
            return orderResponses.ToList();
        }

        public async Task<OrderResponse?> UpdateOrder(OrderUpdateRequest orderUpdateRequest)
        {
            if (orderUpdateRequest == null)
            {
                throw new ArgumentNullException(nameof(orderUpdateRequest));
            }
            ValidationResult orderUpdateRequestValidationResult = await _orderUpdateRequestValidator.ValidateAsync(orderUpdateRequest);
            if (!orderUpdateRequestValidationResult.IsValid)
            {
                string errors = string.Join(", ", orderUpdateRequestValidationResult.Errors.Select(e => e.ErrorMessage));
                throw new ValidationException(errors);
            }
            List<ProductDTO?> products = new List<ProductDTO?>();
            foreach (OrderItemUpdateRequest orderItemUpdateRequest in orderUpdateRequest.OrderItems)
            {
                ValidationResult orderItemUpdateRequestValidationResult = await _orderItemUpdateRequestValidator.ValidateAsync(orderItemUpdateRequest);
                if (!orderItemUpdateRequestValidationResult.IsValid)
                {
                    string errors = string.Join(", ", orderItemUpdateRequestValidationResult.Errors.Select(e => e.ErrorMessage));
                    throw new ValidationException(errors);
                }
                ProductDTO product = await _productsMicroserviceClient.GetProductById(orderItemUpdateRequest.ProductID);
                if (product == null)
                {
                    throw new Exception($"Product with ID {orderItemUpdateRequest.ProductID} not found.");
                }
                products.Add(product);
            }
            UserDTO user = await _usersMicroserviceClient.GetUserById(orderUpdateRequest.UserID);
            if (user == null)
            {
                throw new Exception($"User with ID {orderUpdateRequest.UserID} not found.");
            }

            var orderUpdateEntity = _mapper.Map<Order>(orderUpdateRequest);
            foreach (var orderupdateItem in orderUpdateEntity.OrderItems)
            {
                orderupdateItem.TotalPrice = orderupdateItem.Quantity * orderupdateItem.UnitPrice;
            }
            orderUpdateEntity.TotalBill = orderUpdateEntity.OrderItems.Sum(oi => oi.TotalPrice);
            var addedUpdateOrderEntity = await _ordersRepository.UpdateOrder(orderUpdateEntity);
            var addedUpdateOrderResponse = _mapper.Map<OrderResponse>(addedUpdateOrderEntity);
            if (addedUpdateOrderResponse != null)
            {
                foreach (OrderItemResponse orderItem in addedUpdateOrderResponse.OrderItems)
                {
                    ProductDTO productDTO = products.Where(temp=>temp.ProductID == orderItem.ProductID).FirstOrDefault();
                    if (productDTO == null) { continue; }
                    _mapper.Map<ProductDTO, OrderItemResponse>(productDTO, orderItem);

                }
            }

            return addedUpdateOrderResponse;
        }
    }
}
