using eCommerce.OrdersService.BusinessLogicLayer.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace eCommerce.OrdersService.BusinessLogicLayer.Validator
{
    public class OrderItemUpdateRequestValidator : AbstractValidator<OrderItemUpdateRequest>
    {
        public OrderItemUpdateRequestValidator()
        {
            RuleFor(x => x.ProductID).NotEmpty().WithMessage("Product ID is required.");
            RuleFor(x => x.Quantity).NotEmpty().WithMessage("Unit Price cannot be empty.")
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
            RuleFor(x => x.UnitPrice).NotEmpty().WithMessage("Unit Price cannot be empty.")
                .GreaterThan(0).WithMessage("Unit Price must be greater than 0.");
        }
    }
}
