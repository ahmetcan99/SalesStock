using FluentValidation;
using SalesStock.Application.Features.Orders.DTOs;
using SalesStock.Application.Interfaces;
using SalesStock.Domain.Enums;

public class AddOrderItemDTOValidator : AbstractValidator<AddOrderItemDTO>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public AddOrderItemDTOValidator(IOrderRepository orderRepository, IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;

        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("Order ID is required.");

        RuleFor(x => x.ProductSKU)
            .NotEmpty().WithMessage("You must select a product.")
            .MustAsync(ProductMustExist).WithMessage("The selected product SKU does not exist.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        RuleFor(x => x)
            .MustAsync(OrderMustBeInDraftStatus)
            .WithMessage("Items can only be added to orders in 'Draft' status.");
    }

    private async Task<bool> ProductMustExist(string SKU, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(SKU)) return false;
        var product = await _productRepository.GetBySKUAsync(SKU);
        return product != null;
    }

    private async Task<bool> OrderMustBeInDraftStatus(AddOrderItemDTO dto, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(dto.OrderId);
        return order != null && order.Status == OrderStatus.Draft;
    }
}