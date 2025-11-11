using FluentValidation;

namespace SalesStock.Application.Features.Reports.Validator
{
    public class ReportValidator : AbstractValidator<(DateTime Date, int threshold)>
    {
        public ReportValidator()
        {
            RuleFor(x => x.Date)
                .LessThanOrEqualTo(DateTime.Today)
                .WithMessage("The report date cannot be in the future.");
            RuleFor(x => x.threshold)
                .GreaterThanOrEqualTo(0)
                .WithMessage("The stock threshold must be a non-negative value.");
        }
    }
}
