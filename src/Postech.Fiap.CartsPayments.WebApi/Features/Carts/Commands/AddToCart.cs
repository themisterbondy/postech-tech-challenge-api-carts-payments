using FluentValidation;
using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Contracts;
using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Services;
using Postech.Fiap.CartsPayments.WebApi.Features.Products.Entities;
using Postech.Fiap.CartsPayments.WebApi.Features.Products.Services;

namespace Postech.Fiap.CartsPayments.WebApi.Features.Carts.Commands;

public abstract class AddItensToCart
{
    public class Command : IRequest<Result<CartResponse>>
    {
        public string CustomerId { get; set; }
        public List<CartItemRequest> Items { get; set; }
    }

    public class AddToCartValidator : AbstractValidator<Command>
    {
        public AddToCartValidator()
        {
            When(x => !string.IsNullOrEmpty(x.CustomerId),
                () =>
                {
                    RuleFor(x => x.CustomerId)
                        .NotEmpty().WithError(Error.Validation("CPF", "CPF is required."))
                        .Matches("^[0-9]*$").WithError(Error.Validation("CPF", "CPF must contain only numbers."))
                        .Length(11).WithError(Error.Validation("CPF", "CPF must have 11 characters."))
                        .Must(GlobalValidations.BeAValidCpf).WithMessage("CPF is invalid.");
                });

            RuleForEach(x => x.Items).ChildRules(items =>
            {
                items.RuleFor(i => i.ProductId)
                    .NotEmpty().WithError(Error.Validation("ProductId", "ProductId is required."));

                items.RuleFor(i => i.Quantity)
                    .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
            });
        }
    }

    public class Handler(ICartService cartService, IProductHttpClient productHttpClient)
        : IRequestHandler<Command, Result<CartResponse>>
    {
        public async Task<Result<CartResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            var cartItems = new List<CartItemDto>();

            foreach (var item in request.Items)
            {
                var product = await productHttpClient.FindByIdAsync(new ProductId(item.ProductId), cancellationToken);

                if (product.IsFailure)
                    return Result.Failure<CartResponse>(product.Error);

                cartItems.Add(new CartItemDto
                {
                    ProductId =  product.Value.Id,
                    ProductName = product.Value.Name,
                    UnitPrice = product.Value.Price,
                    Quantity = item.Quantity,
                    Category = product.Value.Category
                });
            }

            return await cartService.AddToCartAsync(request.CustomerId, cartItems);
        }
    }
}