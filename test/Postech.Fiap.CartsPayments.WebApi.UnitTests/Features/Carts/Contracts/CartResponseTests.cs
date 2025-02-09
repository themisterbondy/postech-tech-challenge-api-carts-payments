using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Contracts;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Emun;

namespace Postech.Fiap.CartsPayments.WebApi.UnitTests.Features.Carts.Contracts;

public class CartResponseTests
{
    [Fact]
    public void ShouldInitializeCorrectly()
    {
        var id = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var items = new List<CartItemDto>
        {
            new() { ProductId = Guid.NewGuid(), ProductName = "Product 1", UnitPrice = 10.99m, Quantity = 1 }
        };

        var response = new CartResponse
        {
            CartId = id, CustomerId = customerId, Items = items,
            TotalAmount = 10.99m,
            PaymentStatus = PaymentStatus.Accepted
        };

        response.CartId.Should().Be(id);
        response.CustomerId.Should().Be(customerId);
        response.Items.Should().BeEquivalentTo(items);
        response.Items.Should().HaveCount(1);
        response.Items[0].ProductId.Should().Be(items[0].ProductId);
        response.Items[0].ProductName.Should().Be(items[0].ProductName);
        response.Items[0].UnitPrice.Should().Be(items[0].UnitPrice);
        response.Items[0].Quantity.Should().Be(items[0].Quantity);
        response.TotalAmount.Should().Be(10.99m);
        response.PaymentStatus.Should().Be(PaymentStatus.Accepted);
    }

    [Fact]
    public void ShouldGetAndSetPropertiesCorrectly()
    {
        var response = new CartResponse();

        var id = Guid.NewGuid();
        var customerId = Guid.NewGuid();

        var items = new List<CartItemDto>
        {
            new() { ProductId = Guid.NewGuid(), ProductName = "Product 1", UnitPrice = 10.99m, Quantity = 1 }
        };

        response.CartId = id;
        response.CustomerId = customerId;
        response.Items = items;
        response.TotalAmount = 10.99m;
        response.PaymentStatus = PaymentStatus.Accepted;

        response.CartId.Should().Be(id);
        response.CustomerId.Should().Be(customerId);
        response.Items.Should().HaveCount(1);
        response.Items[0].ProductId.Should().Be(items[0].ProductId);
        response.Items[0].ProductName.Should().Be(items[0].ProductName);
        response.Items[0].UnitPrice.Should().Be(items[0].UnitPrice);
        response.Items[0].Quantity.Should().Be(items[0].Quantity);
        response.TotalAmount.Should().Be(10.99m);
        response.PaymentStatus.Should().Be(PaymentStatus.Accepted);
    }
}