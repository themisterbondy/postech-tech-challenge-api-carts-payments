using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Entities;

namespace Postech.Fiap.CartsPayments.WebApi.UnitTests.Mocks;

public static class CartMocks
{
    public static Cart GenerateValidCart()
    {
        return Cart.Create(CartId.New(), Guid.NewGuid());
    }

    public static Cart GenerateOldCart()
    {
        var cart = Cart.Create(CartId.New(), Guid.NewGuid());
        cart.CreatedAt = DateTime.UtcNow.AddDays(-31);
        return cart;
    }
}