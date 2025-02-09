using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Entities;
using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Repositories;
using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Services;
using Postech.Fiap.CartsPayments.WebApi.Features.Orders.Contracts;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Emun;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Messaging.Queues;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Notifications;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Services;

namespace Postech.Fiap.CartsPayments.WebApi.UnitTests.Features.Payments.Services;

public class PaymentServiceTests
{
    private readonly ICartRepository _cartRepository;
    private readonly ICartService _cartService;
    private readonly PaymentService _paymentService;
    private readonly ICreateOrderCommandSubmittedQueueClient _queueClient;

    public PaymentServiceTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _cartService = Substitute.For<ICartService>();
        _queueClient = Substitute.For<ICreateOrderCommandSubmittedQueueClient>();
        _paymentService = new PaymentService(_cartRepository, _queueClient, _cartService);
    }

    [Fact]
    public async Task InitiatePaymentAsync_Should_Return_Valid_Response()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var amount = 100.50m;

        // Act
        var result = await _paymentService.InitiatePaymentAsync(cartId, amount);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TransactionId.Should().NotBeNullOrEmpty();
        result.Value.QrCodeImageUrl.Should().Contain(result.Value.TransactionId);
    }

    [Fact]
    public async Task GetPaymentStatusAsync_Should_Return_Status_When_Cart_Exists()
    {
        // Arrange
        var cartId = new CartId(Guid.NewGuid());
        var cart = Cart.Create(cartId, Guid.NewGuid());

        cart.PaymentStatus = PaymentStatus.Accepted;
        cart.TransactionId = "TX123";

        _cartRepository.GetByIdAsync(cartId).Returns(Task.FromResult(cart));

        // Act
        var result = await _paymentService.GetPaymentStatusAsync(cartId.Value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CartId.Should().Be(cart.Id.Value);
        result.Value.Status.Should().Be(PaymentStatus.Accepted);
        result.Value.TransactionId.Should().Be("TX123");
    }

    [Fact]
    public async Task GetPaymentStatusAsync_Should_Return_Failure_When_Cart_Not_Found()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        _cartRepository.GetByIdAsync(Arg.Any<CartId>()).Returns(Task.FromResult<Cart>(null));

        // Act
        var result = await _paymentService.GetPaymentStatusAsync(cartId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("Cart not found");
    }

    [Fact]
    public async Task ProcessPaymentNotificationAsync_Should_Create_Order_And_Clear_Cart_When_Payment_Accepted()
    {
        // Arrange
        var transactionId = "TX123";
        var cartId = new CartId(Guid.NewGuid());
        var cart = Cart.Create(cartId, Guid.NewGuid());
        cart.TransactionId = transactionId;
        var notification = new PaymentNotification
        {
            TransactionId = transactionId,
            Status = PaymentStatus.Accepted,
            Amount = 60.00m
        };

        _cartRepository.GetByTransactionIdAsync(transactionId).Returns(Task.FromResult(cart));

        // Act
        var result = await _paymentService.ProcessPaymentNotificationAsync(notification, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        await _queueClient.Received(1).PublishAsync(Arg.Any<CreateOrderCommand>(), Arg.Any<CancellationToken>());
        await _cartService.Received(1).ClearCartAsync(cart.Id.Value);
    }

    [Fact]
    public async Task ProcessPaymentNotificationAsync_Should_Update_Status_When_Payment_Rejected()
    {
        // Arrange
        var transactionId = "TX123";
        var cartId = new CartId(Guid.NewGuid());
        var cart = Cart.Create(cartId, Guid.NewGuid());
        cart.TransactionId = transactionId;

        var notification = new PaymentNotification
        {
            TransactionId = transactionId,
            Status = PaymentStatus.Rejected,
            Amount = 60.00m
        };

        _cartRepository.GetByTransactionIdAsync(transactionId).Returns(Task.FromResult(cart));

        // Act
        var result = await _paymentService.ProcessPaymentNotificationAsync(notification, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        cart.PaymentStatus.Should().Be(PaymentStatus.Rejected);
        await _cartRepository.Received(1).UpdateStatusAsync(cart);
    }

    [Fact]
    public async Task ProcessPaymentNotificationAsync_Should_Return_Failure_When_Cart_Not_Found()
    {
        // Arrange
        var notification = new PaymentNotification
        {
            TransactionId = "TX123",
            Status = PaymentStatus.Accepted,
            Amount = 60.00m
        };

        _cartRepository.GetByTransactionIdAsync(notification.TransactionId).Returns(Task.FromResult<Cart>(null));

        // Act
        var result = await _paymentService.ProcessPaymentNotificationAsync(notification, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("Cart not found or already processed");
    }
}