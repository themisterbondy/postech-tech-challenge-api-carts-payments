using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Contracts;

namespace Postech.Fiap.CartsPayments.WebApi.UnitTests.Features.Payments.Contracts;

public class PaymentInitiationResponseTests
{
    [Fact]
    public void PaymentInitiationResponse_Should_Set_And_Get_Properties_Correctly()
    {
        // Arrange
        var transactionId = Guid.NewGuid().ToString();
        var qrCodeImageUrl = "https://example.com/qrcode.png";

        // Act
        var response = new PaymentInitiationResponse
        {
            TransactionId = transactionId,
            QrCodeImageUrl = qrCodeImageUrl
        };

        // Assert
        response.TransactionId.Should().Be(transactionId);
        response.QrCodeImageUrl.Should().Be(qrCodeImageUrl);
    }

    [Fact]
    public void PaymentInitiationResponse_Should_Allow_Null_Values()
    {
        // Act
        var response = new PaymentInitiationResponse
        {
            TransactionId = null,
            QrCodeImageUrl = null
        };

        // Assert
        response.TransactionId.Should().BeNull();
        response.QrCodeImageUrl.Should().BeNull();
    }
}