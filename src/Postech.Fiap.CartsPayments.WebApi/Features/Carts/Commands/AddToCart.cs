using FluentValidation;
using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Contracts;
using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Services;
using Postech.Fiap.CartsPayments.WebApi.Features.Products.Entities;
using Postech.Fiap.CartsPayments.WebApi.Features.Products.Repositories;

namespace Postech.Fiap.CartsPayments.WebApi.Features.Carts.Commands;

public class AddItensToCart
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

    public class Handler(ICartService cartService, IProductRepository productRepository)
        : IRequestHandler<Command, Result<CartResponse>>
    {
        public async Task<Result<CartResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            var cartItems = new List<CartItemDto>();

            foreach (var item in request.Items)
            {
                var product = await productRepository.FindByIdAsync(new ProductId(item.ProductId), cancellationToken);

                if (product == null)
                {
                    return Result.Failure<CartResponse>(Error.NotFound("AddToCart.Handler",
                        $"Product with ID {item.ProductId} not found."));
                }

                cartItems.Add(new CartItemDto
                {
                    ProductId = product.Id.Value,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = item.Quantity,
                    Category = product.Category
                });
            }

            return await cartService.AddToCartAsync(request.CustomerId, cartItems);
        }
    }
}