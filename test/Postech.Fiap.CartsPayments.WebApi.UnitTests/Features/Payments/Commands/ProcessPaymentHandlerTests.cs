using Postech.Fiap.CartsPayments.WebApi.Common.ResultPattern;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Commands;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Emun;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Notifications;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Services;

namespace Postech.Fiap.CartsPayments.WebApi.UnitTests.Features.Payments.Commands;

public class ProcessPaymentHandlerTests
{
    private readonly ProcessPayment.Handler _handler;
    private readonly IPaymentService _paymentService;

    public ProcessPaymentHandlerTests()
    {
        _paymentService = Substitute.For<IPaymentService>();
        _handler = new ProcessPayment.Handler(_paymentService);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_When_Payment_Is_Processed()
    {
        // Arrange
        var transactionId = Guid.NewGuid().ToString();
        var status = PaymentStatus.Accepted;
        var amount = 100.50m;

        var command = new ProcessPayment.Command
        {
            TransactionId = transactionId,
            Status = status,
            Amount = amount
        };

        _paymentService.ProcessPaymentNotificationAsync(Arg.Any<PaymentNotification>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TransactionId.Should().Be(transactionId);
        result.Value.Status.Should().Be(status);
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_PaymentService_Fails()
    {
        // Arrange
        var transactionId = Guid.NewGuid().ToString();
        var status = PaymentStatus.Rejected;
        var amount = 50.00m;
        var error = Error.Failure("PaymentService.ProcessPaymentNotificationAsync", "Payment processing failed");

        var command = new ProcessPayment.Command
        {
            TransactionId = transactionId,
            Status = status,
            Amount = amount
        };

        _paymentService.ProcessPaymentNotificationAsync(Arg.Any<PaymentNotification>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }
}