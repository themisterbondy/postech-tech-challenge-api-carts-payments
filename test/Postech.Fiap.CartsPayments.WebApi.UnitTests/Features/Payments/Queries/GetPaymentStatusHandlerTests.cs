using Postech.Fiap.CartsPayments.WebApi.Common.ResultPattern;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Contracts;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Emun;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Queries;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Services;

namespace Postech.Fiap.CartsPayments.WebApi.UnitTests.Features.Payments.Queries;

public class GetPaymentStatusHandlerTests
{
    private readonly GetPaymentStatus.Handler _handler;
    private readonly IPaymentService _paymentService;

    public GetPaymentStatusHandlerTests()
    {
        _paymentService = Substitute.For<IPaymentService>();
        _handler = new GetPaymentStatus.Handler(_paymentService);
    }

    [Fact]
    public async Task Handle_Should_Return_PaymentStatusResponse_When_Successful()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var expectedResponse = new PaymentStatusResponse
        {
            CartId = cartId,
            Status = PaymentStatus.Accepted,
            TransactionId = "TX123"
        };

        _paymentService.GetPaymentStatusAsync(cartId)
            .Returns(Result.Success(expectedResponse));

        var query = new GetPaymentStatus.Query { CartId = cartId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_PaymentService_Fails()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var error = Error.Failure("PaymentService.GetPaymentStatusAsync", "Cart not found");

        _paymentService.GetPaymentStatusAsync(cartId)
            .Returns(Result.Failure<PaymentStatusResponse>(error));

        var query = new GetPaymentStatus.Query { CartId = cartId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }
}