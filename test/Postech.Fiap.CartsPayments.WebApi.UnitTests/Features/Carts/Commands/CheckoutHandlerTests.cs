using Postech.Fiap.CartsPayments.WebApi.Common.ResultPattern;
using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Commands;
using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Entities;
using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Repositories;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Contracts;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Emun;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Services;
using Postech.Fiap.CartsPayments.WebApi.Features.Products.Entities;
using Postech.Fiap.CartsPayments.WebApi.UnitTests.Mocks;

namespace Postech.Fiap.CartsPayments.WebApi.UnitTests.Features.Carts.Commands;

public class CheckoutHandlerTests
{
    private readonly ICartRepository _cartRepository = Substitute.For<ICartRepository>();
    private readonly Checkout.Handler _handler;
    private readonly IPaymentService _paymentService = Substitute.For<IPaymentService>();

    public CheckoutHandlerTests()
    {
        _handler = new Checkout.Handler(_cartRepository, _paymentService);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_CartNotFound()
    {
        // Arrange
        var command = new Checkout.Command { CartId = Guid.NewGuid() };
        _cartRepository.GetByIdAsync(Arg.Any<CartId>()).Returns((Cart)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("Cart is empty or not found.");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_CartIsEmpty()
    {
        // Arrange
        var cart = Cart.Create(new CartId(Guid.NewGuid()), Guid.NewGuid());
        _cartRepository.GetByIdAsync(Arg.Any<CartId>()).Returns(cart);

        var command = new Checkout.Command { CartId = cart.Id.Value };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("Cart is empty or not found.");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_CartAlreadyCheckedOut()
    {
        // Arrange
        var cart = Cart.Create(new CartId(Guid.NewGuid()), Guid.NewGuid());
        cart.PaymentStatus = PaymentStatus.Pending;
        var cartItem = CartItem.Create(CartItemId.New(), new ProductId(Guid.NewGuid()), "Product", 10m, 2,
            ProductCategory.Lanche);
        cart.AddItem(cartItem);

        var db = ApplicationDbContextMock.Create();
        db.Carts.Add(cart);
        await db.SaveChangesAsync();


        _cartRepository.GetByIdAsync(Arg.Any<CartId>()).Returns(cart);

        var command = new Checkout.Command { CartId = cart.Id.Value };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("Cart has already been checked out");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_PaymentInitiationFails()
    {
        // Arrange
        var cart = Cart.Create(new CartId(Guid.NewGuid()), Guid.NewGuid());
        var cartItem = CartItem.Create(CartItemId.New(), new ProductId(Guid.NewGuid()), "Product", 10m, 2,
            ProductCategory.Lanche);
        cart.AddItem(cartItem);

        var db = ApplicationDbContextMock.Create();
        db.Carts.Add(cart);
        await db.SaveChangesAsync();

        _cartRepository.GetByIdAsync(Arg.Any<CartId>()).Returns(cart);

        _paymentService.InitiatePaymentAsync(Arg.Any<Guid>(), Arg.Any<decimal>())
            .Returns(Task.FromResult(Result.Failure<PaymentInitiationResponse>(Error.Failure("dd", "aa"))));

        var command = new Checkout.Command { CartId = cart.Id.Value };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("Failed to initiate payment.");
    }

    [Fact]
    public async Task Handle_Should_ProcessCheckout_When_Valid()
    {
        // Arrange
        var cart = Cart.Create(new CartId(Guid.NewGuid()), Guid.NewGuid());
        var cartItem = CartItem.Create(CartItemId.New(), new ProductId(Guid.NewGuid()), "Product", 10m, 2,
            ProductCategory.Lanche);
        cart.AddItem(cartItem);

        var db = ApplicationDbContextMock.Create();
        db.Carts.Add(cart);
        await db.SaveChangesAsync();

        var paymentResult = new PaymentInitiationResponse
            { TransactionId = "123", QrCodeImageUrl = "https://qrcode.com/payment" };

        _cartRepository.GetByIdAsync(Arg.Any<CartId>()).Returns(cart);
        _paymentService.InitiatePaymentAsync(Arg.Any<Guid>(), Arg.Any<decimal>())
            .Returns(Task.FromResult(Result.Success(paymentResult)));

        var command = new Checkout.Command { CartId = cart.Id.Value };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CartId.Should().Be(cart.Id.Value);
        result.Value.TotalAmount.Should().Be(20m);
        result.Value.QrCodeImageUrl.Should().Be(paymentResult.QrCodeImageUrl);
        result.Value.TransactionId.Should().Be(paymentResult.TransactionId);

        await _cartRepository.Received(1).UpdateStatusAsync(cart);
        await _paymentService.Received(1).InitiatePaymentAsync(cart.Id.Value, 20m);
    }
}