using FluentValidation;
using SalesStock.Application.Interfaces;
using SalesStock.Application.Features.Customers.DTOs;

namespace SalesStock.Application.Features.Customers.Validators
{
    public class CreateCustomerDTOValidator : AbstractValidator<CreateCustomerDTO>
    {
        private readonly ICustomerRepository _customerRepository;
        public CreateCustomerDTOValidator(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
            RuleFor(c => c.Code)
                .NotEmpty().WithMessage("Customer code is required.")
                .MaximumLength(20).WithMessage("Customer code must not exceed 20 characters.")
                .MustAsync(BeUniqueCode).WithMessage("Customer code must be unique.");
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Customer name is required.")
                .MaximumLength(100).WithMessage("Customer name must not exceed 100 characters.");
            RuleFor(c => c.Email)
                .EmailAddress().WithMessage("A valid email address is required.")
                .When(c => !string.IsNullOrEmpty(c.Email));
            RuleFor(c => c.Phone)
                .MaximumLength(15).WithMessage("Phone number must not exceed 15 characters.")
                .When(c => !string.IsNullOrEmpty(c.Phone));
        }

        private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
        {
            return !await _customerRepository.CodeExistsAsync(code);
        }
    }
}
