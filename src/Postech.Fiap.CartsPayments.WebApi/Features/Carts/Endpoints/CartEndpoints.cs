using Microsoft.AspNetCore.Mvc;
using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Commands;
using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Contracts;
using Postech.Fiap.CartsPayments.WebApi.Features.Orders.Contracts;

namespace Postech.Fiap.CartsPayments.WebApi.Features.Carts.Endpoints;

[ExcludeFromCodeCoverage]
public class CartEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/carts");

        group.MapPost("/", async (CartRequest request, ISender sender) =>
            {
                var command = new AddItensToCart.Command
                {
                    CustomerId = request.CustomerId,
                    Items = request.Items
                };

                var result = await sender.Send(command);

                return result.IsSuccess
                    ? Results.Created($"/carts/{result.Value.CartId}", result.Value)
                    : result.ToProblemDetails();
            })
            .WithName("AddToCart")
            .Accepts<CartRequest>("application/json")
            .Produces<CartResponse>(201)
            .WithTags("Carts")
            .WithOpenApi();

        group.MapPost("/checkout",
                async ([FromQuery] Guid cartId, ISender sender) =>
                {
                    var result = await sender.Send(new Checkout.Command { CartId = cartId });
                    return result.IsSuccess
                        ? Results.Created($"/order/{result.Value.CartId}", result.Value)
                        : result.ToProblemDetails();
                })
            .WithName("Checkout")
            .Produces<CheckoutResponse>(201)
            .WithTags("Carts")
            .WithOpenApi();
    }
}