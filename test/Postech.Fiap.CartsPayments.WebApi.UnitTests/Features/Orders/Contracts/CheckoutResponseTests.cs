using Postech.Fiap.CartsPayments.WebApi.Features.Orders.Contracts;

namespace Postech.Fiap.CartsPayments.WebApi.UnitTests.Features.Orders.Contracts;

public class CheckoutResponseTests
{
    [Fact]
    public void CheckoutResponse_Should_Have_Correct_Properties()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var status = "Completed";
        var totalAmount = 150.75m;
        var items = new List<OrderItemDto>
        {
            new() { ProductId = Guid.NewGuid(), Quantity = 2, UnitPrice = 50.25m },
            new() { ProductId = Guid.NewGuid(), Quantity = 1, UnitPrice = 50.25m }
        };
        var qrCodeImageUrl = "https://example.com/qrcode.png";
        var transactionId = Guid.NewGuid().ToString();

        // Act
        var response = new CheckoutResponse
        {
            CartId = cartId,
            CustomerId = customerId,
            Status = status,
            TotalAmount = totalAmount,
            Items = items,
            QrCodeImageUrl = qrCodeImageUrl,
            TransactionId = transactionId
        };

        // Assert
        response.CartId.Should().Be(cartId);
        response.CustomerId.Should().Be(customerId);
        response.Status.Should().Be(status);
        response.TotalAmount.Should().Be(totalAmount);
        response.Items.Should().BeEquivalentTo(items);
        response.QrCodeImageUrl.Should().Be(qrCodeImageUrl);
        response.TransactionId.Should().Be(transactionId);
    }

    [Fact]
    public void CheckoutResponse_Should_Initialize_Empty_List_When_Null()
    {
        // Act
        var response = new CheckoutResponse
        {
            Items = null // Simulando um valor nulo para a lista
        };

        // Assert
        response.Items.Should().BeNull();
    }
}