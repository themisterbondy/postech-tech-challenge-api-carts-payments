using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Contracts;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Emun;

namespace Postech.Fiap.CartsPayments.WebApi.UnitTests.Features.Payments.Contracts;

public class PaymentStatusResponseTests
{
    [Fact]
    public void PaymentStatusResponse_Should_Set_And_Get_Properties_Correctly()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var status = PaymentStatus.Accepted;
        var transactionId = Guid.NewGuid().ToString();

        // Act
        var response = new PaymentStatusResponse
        {
            CartId = cartId,
            Status = status,
            TransactionId = transactionId
        };

        // Assert
        response.CartId.Should().Be(cartId);
        response.Status.Should().Be(status);
        response.TransactionId.Should().Be(transactionId);
    }

    [Fact]
    public void PaymentStatusResponse_Should_Allow_Null_TransactionId()
    {
        // Act
        var response = new PaymentStatusResponse
        {
            CartId = Guid.NewGuid(),
            Status = PaymentStatus.Pending,
            TransactionId = null
        };

        // Assert
        response.TransactionId.Should().BeNull();
    }
}