namespace DeleteOrder.Validators;

internal sealed class DeleteOrderRequestValidator : AbstractValidator<DeleteOrderRequest>
{
    public DeleteOrderRequestValidator()
    {
        RuleFor(request => request.CustomerId)
            .NotEmpty()
            .WithMessage("customerId is required.");

        RuleFor(request => request.OrderId)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithMessage("orderId is required.");
    }
}
