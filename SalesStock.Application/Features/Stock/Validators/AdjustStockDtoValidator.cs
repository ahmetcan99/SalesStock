using FluentValidation;
using SalesStock.Application.Features.Stock.DTOs;

namespace SalesStock.Application.Features.Stock.Validators
{
    public class AdjustStockDtoValidator : AbstractValidator<AdjustStockDTO>
    {
        public AdjustStockDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("You must select a product.");
            RuleFor(x => x.Quantity)
                .NotEqual(0).WithMessage("Quantity must be non-zero.");
            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("Reason is required.")
                .MaximumLength(200).WithMessage("Reason cannot exceed 200 characters.");
        }
    }
}
