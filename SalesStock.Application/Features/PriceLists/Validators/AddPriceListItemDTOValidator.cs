using FluentValidation;
using SalesStock.Application.Features.PriceLists.DTOs;

namespace SalesStock.Application.Features.PriceLists.Validators
{
    public class AddPriceListItemDTOValidator : AbstractValidator<AddPriceListItemDTO>
    {
        public AddPriceListItemDTOValidator()
        {
            RuleFor(x => x.PriceListId)
                .NotEmpty().WithMessage("Price List ID is required.");
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Id is required.");
            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");
            RuleFor(x => x.ValidFrom)
                .LessThan(x => x.ValidTo).WithMessage("Valid From date must be earlier than Valid To date.");
            RuleFor(x => x.ValidTo)
                .GreaterThan(x => x.ValidFrom).WithMessage("Valid To date must be later than Valid From date.");
        }

    }
}
