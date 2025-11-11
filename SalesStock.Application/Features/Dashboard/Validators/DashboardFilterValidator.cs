using FluentValidation;
using SalesStock.Application.Features.Dashboard.DTOs;

namespace SalesStock.Application.Features.Dashboard.Validators
{
    public class DashboardFilterValidator : AbstractValidator<DashboardFilterDTO>
    {
        public DashboardFilterValidator()
        {
            RuleFor(x => x.LowStockThreshold).GreaterThanOrEqualTo(0);
            When(x => x.Range == DashboardQuickRange.Custom, () =>
            {
                RuleFor(x => x.StartDate).NotNull();
                RuleFor(x => x.EndDate).NotNull();
                RuleFor(x => x).Must(x => x.StartDate <= x.EndDate).WithMessage("StartDate must be before EndDate");
            });
        }
    }
}
