namespace GetOrder.Validators;

internal sealed class GetOrderRequestValidator : AbstractValidator<GetOrderRequest>
{
    public GetOrderRequestValidator()
    {
        RuleFor(request => request.CustomerId)
            .NotEmpty()
            .WithMessage("customerId is required.");

        RuleFor(request => request.OrderId)
            .NotEqual(Guid.Empty)
            .WithMessage("orderId is required.");
    }
}