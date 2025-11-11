using FluentValidation;
using SalesStock.Application.Features.Orders.DTOs;

namespace SalesStock.Application.Features.Orders.Validators
{
    public class CreateOrderDTOValidator : AbstractValidator<CreateOrderDTO>
    {
        public CreateOrderDTOValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("CustomerId is required.");
            RuleFor(x => x.PriceListId)
                .NotEmpty().WithMessage("PriceListId is required.");
        }
    }
}
