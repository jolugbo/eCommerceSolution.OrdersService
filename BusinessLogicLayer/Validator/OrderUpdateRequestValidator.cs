using eCommerce.OrdersService.BusinessLogicLayer.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace eCommerce.OrdersService.BusinessLogicLayer.Validator
{
    public class OrderUpdateRequestValidator : AbstractValidator<OrderUpdateRequest>
    {
        public OrderUpdateRequestValidator()
        {
            RuleFor(x => x.UserID).NotEmpty().WithMessage("User ID is required.");
            RuleFor(x => x.OrderDate).NotEmpty().WithMessage("Order Date is required.");
            RuleFor(x => x.OrderItems).NotEmpty().WithMessage("Order Items is required.");
        }
    }
}
