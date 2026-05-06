namespace UpdateOrder.Validators;

internal sealed class UpdateOrderRequestValidator : AbstractValidator<UpdateOrderRequest>
{
    public UpdateOrderRequestValidator()
    {
        RuleFor(request => request.CustomerId)
            .NotEmpty()
            .WithMessage("customerId is required.");

        RuleFor(request => request.OrderId)
            .NotEqual(Guid.Empty)
            .WithMessage("orderId is required.");

        RuleFor(request => request.Currency)
            .NotEmpty()
            .WithMessage("currency is required.");

        RuleFor(request => request.ShippingAddress)
            .NotEmpty()
            .WithMessage("shippingAddress is required.");

        RuleFor(request => request.Items)
            .NotEmpty()
            .WithMessage("At least one item is required.");

        RuleForEach(r => r.Items)
            .ChildRules(item =>
            {
                item.RuleFor(i => i.Quantity)
                    .GreaterThan(0)
                    .WithMessage("Each item quantity must be greater than zero.");

                item.RuleFor(i => i.UnitPrice)
                    .GreaterThan(0)
                    .WithMessage("Each item unitPrice must be greater than zero.");
            });
    }
}
