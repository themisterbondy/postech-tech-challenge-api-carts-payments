using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Contracts;

namespace Postech.Fiap.CartsPayments.WebApi.UnitTests.Features.Carts.Contracts;

public class CartRequestTests
{
    [Fact]
    public void ShouldInitializeCorrectly()
    {
        var customerId = Guid.NewGuid();

        var cartItemRequest = new CartItemRequest
        {
            ProductId = Guid.NewGuid(),
            Quantity = 1
        };

        var request = new CartRequest { CustomerId = customerId, Items = [cartItemRequest] };

        request.CustomerId.Should().Be(customerId);
        request.Items.Should().HaveCount(1);
        request.Items[0].ProductId.Should().Be(cartItemRequest.ProductId);
        request.Items[0].Quantity.Should().Be(cartItemRequest.Quantity);
    }

    [Fact]
    public void ShouldGetAndSetPropertiesCorrectly()
    {
        var request = new CartRequest();

        var customerId = Guid.NewGuid();
        var cartItemRequest = new CartItemRequest
        {
            ProductId = Guid.NewGuid(),
            Quantity = 1
        };

        request.CustomerId = customerId;
        request.Items = [cartItemRequest];

        request.CustomerId.Should().Be(customerId);
        request.Items.Should().HaveCount(1);
    }
}