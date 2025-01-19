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
        public Guid CustomerId { get; set; }
        public List<CartItemRequest> Items { get; set; }
    }

    public class AddToCartValidator : AbstractValidator<Command>
    {
        public AddToCartValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty().WithError(Error.Validation("CustomerId", "CustomerId is required."))
                .Must(id => Guid.TryParse(id.ToString(), out _))
                .WithError(Error.Validation("CustomerId", "CustomerId must be a valid GUID."));

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
                    ProductId = product.Value.Id,
                    ProductName = product.Value.Name,
                    UnitPrice = product.Value.Price,
                    Quantity = item.Quantity,
                    Category = product.Value.Category
                });
            }

            var cartServiceResult = await cartService.AddToCartAsync(request.CustomerId, cartItems);
            return cartServiceResult.IsFailure
                ? Result.Failure<CartResponse>(cartServiceResult.Error)
                : Result.Success(cartServiceResult.Value);
        }
    }
}