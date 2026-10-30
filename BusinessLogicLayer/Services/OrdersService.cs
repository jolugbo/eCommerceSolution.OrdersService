using AutoMapper;
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

        public OrderService(IOrdersRepository ordersRepository, IMapper mapper, OrderAddRequestValidator orderAddRequestValidator, IValidator<OrderItemAddRequest> orderItemAddRequestValidator,
            IValidator<OrderUpdateRequest> orderUpdateRequestValidator, IValidator<OrderItemUpdateRequest> orderItemUpdateRequestValidator)
        {
            _ordersRepository = ordersRepository;
            _mapper = mapper;
            _orderAddRequestValidator = orderAddRequestValidator;
            _orderItemAddRequestValidator = orderItemAddRequestValidator;
            _orderUpdateRequestValidator = orderUpdateRequestValidator;
            _orderItemUpdateRequestValidator = orderItemUpdateRequestValidator;
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
            foreach ( OrderItemAddRequest orderItemAddRequest in orderAddRequest.OrderItems)
            {
                ValidationResult orderItemAddRequestValidationResult = await _orderItemAddRequestValidator.ValidateAsync(orderItemAddRequest);
                if (!orderItemAddRequestValidationResult.IsValid)
                {
                    string errors = string.Join(", ", orderItemAddRequestValidationResult.Errors.Select(e => e.ErrorMessage));
                    throw new ValidationException(errors);
                }
            }
            var orderEntity = _mapper.Map<Order>(orderAddRequest);
            foreach (var orderItem in orderEntity.OrderItems)
            {
                orderItem.TotalPrice = orderItem.Quantity * orderItem.UnitPrice;
            }
            orderEntity.TotalBill = orderEntity.OrderItems.Sum(oi => oi.TotalPrice);
            var addedOrderEntity = await _ordersRepository.AddOrder(orderEntity);
            var addedOrderResponse = _mapper.Map<OrderResponse>(addedOrderEntity);
            return addedOrderResponse;
        }

        public async Task<bool> DeleteOrder(Guid orderID)
        {
            FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(temp => temp.OrderId, orderID);
            Order? existingOrder = await _ordersRepository.GetOrderByCondition(filter);
            if (existingOrder == null) return false;
           bool isDeleted = await _ordersRepository.DeleteOrder(orderID);
            return isDeleted;
        }

        public async Task<OrderResponse?> GetOrderByCondition(FilterDefinition<Order?> filter)
        {
            Order? order = await _ordersRepository.GetOrderByCondition(filter);
            if(order == null) return null;
            return _mapper.Map<OrderResponse>(order);
        }

        public async Task<List<OrderResponse?>> GetOrders()
        {
            IEnumerable<Order>? order = await _ordersRepository.GetOrders();
            if (order == null) return null;
            return _mapper.Map<List<OrderResponse>>(order).ToList();

        }

        public async Task<List<OrderResponse>?> GetOrdersByCondition(FilterDefinition<Order> filter)
        {
            IEnumerable<Order>? order = await _ordersRepository.GetOrdersByCondition(filter);
            if (order == null) return null;
            return _mapper.Map<List<OrderResponse>>(order).ToList();
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
            foreach (OrderItemUpdateRequest orderItemUpdateRequest in orderUpdateRequest.OrderItems)
            {
                ValidationResult orderItemUpdateRequestValidationResult = await _orderItemUpdateRequestValidator.ValidateAsync(orderItemUpdateRequest);
                if (!orderItemUpdateRequestValidationResult.IsValid)
                {
                    string errors = string.Join(", ", orderItemUpdateRequestValidationResult.Errors.Select(e => e.ErrorMessage));
                    throw new ValidationException(errors);
                }
            }
            var orderUpdateEntity = _mapper.Map<Order>(orderUpdateRequest);
            foreach (var orderupdateItem in orderUpdateEntity.OrderItems)
            {
                orderupdateItem.TotalPrice = orderupdateItem.Quantity * orderupdateItem.UnitPrice;
            }
            orderUpdateEntity.TotalBill = orderUpdateEntity.OrderItems.Sum(oi => oi.TotalPrice);
            var addedUpdateOrderEntity = await _ordersRepository.UpdateOrder(orderUpdateEntity);
            var addedUpdateOrderResponse = _mapper.Map<OrderResponse>(addedUpdateOrderEntity);
            return addedUpdateOrderResponse;
        }
    }
}
