using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Emun;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Notifications;

namespace Postech.Fiap.CartsPayments.WebApi.UnitTests.Features.Payments.Notifications;

public class PaymentNotificationTests
{
    [Fact]
    public void PaymentNotification_Should_Set_And_Get_Properties_Correctly()
    {
        // Arrange
        var transactionId = Guid.NewGuid().ToString();
        var status = PaymentStatus.Accepted;
        var amount = 100.50m;

        // Act
        var notification = new PaymentNotification
        {
            TransactionId = transactionId,
            Status = status,
            Amount = amount
        };

        // Assert
        notification.TransactionId.Should().Be(transactionId);
        notification.Status.Should().Be(status);
        notification.Amount.Should().Be(amount);
    }

    [Fact]
    public void PaymentNotification_Should_Allow_Empty_TransactionId()
    {
        // Act
        var notification = new PaymentNotification
        {
            TransactionId = string.Empty,
            Status = PaymentStatus.Pending,
            Amount = 50.00m
        };

        // Assert
        notification.TransactionId.Should().BeEmpty();
    }
}