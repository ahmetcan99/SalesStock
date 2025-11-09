using FluentValidation;
using SalesStock.Application.Features.Products.DTOs;

namespace SalesStock.Application.Features.Products.Validators
{
    public class CreateProductDTOValidator : AbstractValidator<CreateProductDTO>
    {
        public CreateProductDTOValidator()
        {
            RuleFor(p => p.SKU)
                .NotEmpty().WithMessage("SKU is required.")
                .MaximumLength(50).WithMessage("SKU cannot exceed 50 characters.");
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(3).WithMessage("Name must be at least 3 characters.");
            RuleFor(p => p.Unit)
                .NotEmpty().WithMessage("Unit is required.")
                .MaximumLength(20).WithMessage("Unit cannot exceed 20 characters.");
            RuleFor(p => p.VatRate)
                .InclusiveBetween(0, 1).WithMessage("VAT Rate must be between 0 and 1.");
            RuleFor(p => p.UnitPrice)
                .GreaterThan(0).WithMessage("Unit Price must be greater than zero.");
            RuleFor(p => p.BarCode)
                .MaximumLength(30).WithMessage("BarCode cannot exceed 30 characters.");
        }
    }
}
