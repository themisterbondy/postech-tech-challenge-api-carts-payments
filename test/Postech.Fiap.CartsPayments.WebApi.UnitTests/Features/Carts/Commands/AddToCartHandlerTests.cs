using Postech.Fiap.CartsPayments.WebApi.Common.ResultPattern;
using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Commands;
using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Contracts;
using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Services;
using Postech.Fiap.CartsPayments.WebApi.Features.Products.Contracts;
using Postech.Fiap.CartsPayments.WebApi.Features.Products.Entities;
using Postech.Fiap.CartsPayments.WebApi.Features.Products.Services;

namespace Postech.Fiap.CartsPayments.WebApi.UnitTests.Features.Carts.Commands;

public class AddToCartHandlerTests
{
    private readonly ICartService _cartService;
    private readonly AddItensToCart.Handler _handler;
    private readonly IProductHttpClient _productHttpClient;

    public AddToCartHandlerTests()
    {
        _cartService = Substitute.For<ICartService>();
        _productHttpClient = Substitute.For<IProductHttpClient>();
        _handler = new AddItensToCart.Handler(_cartService, _productHttpClient);
    }

    [Fact]
    public async Task ShouldReturnFailureWhenProductNotFound()
    {
        _productHttpClient.FindByIdAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<ProductResponse>(
                Error.Failure("AddToCart.Handler", "Product not found")
            )));
        var cartItemRequest = new CartItemRequest { ProductId = Guid.NewGuid(), Quantity = 1 };

        var command = new AddItensToCart.Command { CustomerId = Guid.NewGuid(), Items = [cartItemRequest] };
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("AddToCart.Handler");
    }

    [Fact]
    public async Task ShouldAddToCartSuccessfullyWhenProductIsFound()
    {
        var productResponse = new ProductResponse
        {
            Id = Guid.NewGuid(),
            Name = "Product Name",
            Description = "Product Description",
            Price = 10,
            Category = ProductCategory.Lanche,
            ImageUrl = "http://example.com/image.jpg"
        };
        _productHttpClient.FindByIdAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>())
            .Returns(productResponse);

        var command = new AddItensToCart.Command
        {
            CustomerId = Guid.NewGuid(), Items = [new CartItemRequest { ProductId = productResponse.Id, Quantity = 1 }]
        };
        _cartService
            .AddToCartAsync(Arg.Any<Guid>(), Arg.Any<List<CartItemDto>>())
            .Returns(Task.FromResult(Result.Success(new CartResponse())));


        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }
}