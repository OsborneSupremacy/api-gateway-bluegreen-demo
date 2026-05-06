namespace CreateOrder.Validators;

internal sealed class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
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

