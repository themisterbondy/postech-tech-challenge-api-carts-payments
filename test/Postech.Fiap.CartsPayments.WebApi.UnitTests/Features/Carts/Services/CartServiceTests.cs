using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Contracts;
using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Entities;
using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Repositories;
using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Services;
using Postech.Fiap.CartsPayments.WebApi.Features.Products.Entities;
using Postech.Fiap.CartsPayments.WebApi.UnitTests.Mocks;

namespace Postech.Fiap.CartsPayments.WebApi.UnitTests.Features.Carts.Services;

public class CartServiceTests
{
    private readonly ICartRepository _cartRepository;
    private readonly CartService _cartService;

    public CartServiceTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _cartService = new CartService(_cartRepository);
    }

    [Fact]
    public async Task ShouldAddNewItemToCart()
    {
        var customerId = Guid.NewGuid();
        var cartItem = new CartItemDto
        {
            ProductId = Guid.NewGuid(),
            ProductName = "Test Product",
            UnitPrice = 10.99m,
            Quantity = 1
        };

        _cartRepository.GetByCustomerIdAsync(customerId).Returns((Cart)null);

        var result = await _cartService.AddToCartAsync(customerId, [cartItem]);

        result.Value.Items.Should().ContainSingle(i => i.ProductId == cartItem.ProductId);
        result.Value.Items.Should().ContainSingle(i => i.ProductName == cartItem.ProductName);
    }

    [Fact]
    public async Task ShouldUpdateExistingItemInCart()
    {
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var cartItem = new CartItemDto
        {
            ProductId = productId,
            ProductName = "Test Product",
            UnitPrice = 10.99m,
            Quantity = 1
        };

        var existingCart = Cart.Create(CartId.New(), customerId);
        existingCart.AddItem(CartItem.Create(CartItemId.New(), new ProductId(productId), "Test Product", 10.99m, 1,
            ProductCategory.Lanche));

        var db = ApplicationDbContextMock.Create();
        db.Carts.Add(existingCart);
        await db.SaveChangesAsync();

        _cartRepository.GetByCustomerIdAsync(customerId).Returns(existingCart);
        _cartRepository.ExistsAsync(Arg.Any<CartId>()).Returns(true);

        var result = await _cartService.AddToCartAsync(customerId, [cartItem]);

        result.Value.Items.Should().ContainSingle(i => i.ProductId == productId);
    }

    [Fact]
    public async Task ShouldReturnNullWhenCartNotFoundForGetCartByCustomerId()
    {
        var customerId = Guid.Empty;
        _cartRepository.GetByCustomerIdAsync(customerId).Returns((Cart)null);

        var result = await _cartService.GetCartByCustomerIdAsync(customerId);

        result.Should().BeNull();
    }

    [Fact]
    public async Task ShouldReturnCartWhenFoundForGetCartByCustomerId()
    {
        var customerId = Guid.NewGuid();
        var existingCart = Cart.Create(CartId.New(), customerId);
        var cartItem = CartItem.Create(CartItemId.New(), new ProductId(Guid.NewGuid()), "Test Product", 10.99m, 1,
            ProductCategory.Lanche);
        existingCart.AddItem(cartItem);
        _cartRepository.GetByCustomerIdAsync(customerId).Returns(existingCart);

        var result = await _cartService.GetCartByCustomerIdAsync(customerId);

        result.CartId.Should().Be(existingCart.Id.Value);
        result.CustomerId.Should().Be(customerId);
        result.Items[0].ProductId.Should().Be(existingCart.Items[0].ProductId.Value);
        result.Items[0].ProductName.Should().Be(existingCart.Items[0].ProductName);
        result.Items[0].UnitPrice.Should().Be(existingCart.Items[0].UnitPrice);
        result.Items[0].Quantity.Should().Be(existingCart.Items[0].Quantity);
    }

    [Fact]
    public async Task ShouldRemoveItemFromCart()
    {
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var existingCart = Cart.Create(CartId.New(), customerId);
        var cartItem = CartItem.Create(CartItemId.New(), new ProductId(productId), "Test Product", 10.99m, 1,
            ProductCategory.Lanche);
        existingCart.AddItem(cartItem);
        _cartRepository.GetByCustomerIdAsync(customerId).Returns(existingCart);

        var result = await _cartService.RemoveFromCartAsync(customerId, productId);

        result.Should().BeNull();
        _cartRepository.Received(1).UpdateAsync(existingCart);
    }

    [Fact]
    public async Task ShouldRemoveItemFromCartEmpty()
    {
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var existingCart = Cart.Create(CartId.New(), customerId);
        _cartRepository.GetByCustomerIdAsync(customerId).Returns(existingCart);

        var result = await _cartService.RemoveFromCartAsync(customerId, productId);

        result.Should().BeNull();
    }


    [Fact]
    public async Task ClearCartAsync_ShouldReturnNull_WhenCartNotFound()
    {
        // Arrange
        var cartService = new CartService(_cartRepository);
        var cartId = Guid.NewGuid();

        _cartRepository.GetByIdAsync(Arg.Any<CartId>()).Returns((Cart)null);

        // Act
        var result = await cartService.ClearCartAsync(cartId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ClearCartAsync_ShouldClearItemsAndDeleteCart_WhenCartFound()
    {
        // Arrange
        var cartService = new CartService(_cartRepository);
        var cartId = new CartId(Guid.NewGuid());
        var cart = Cart.Create(cartId, Guid.NewGuid());
        var cartItem = CartItem.Create(CartItemId.New(), ProductId.New(), "Product 1", 10.99m, 1,
            ProductCategory.Lanche);
        cart.Items.Add(cartItem);

        _cartRepository.GetByIdAsync(Arg.Is<CartId>(x => x == cartId)).Returns(cart);

        // Act
        var result = await cartService.ClearCartAsync(cartId.Value);

        // Assert
        result.Should().BeNull();

        // Verifica se os itens foram removidos
        await _cartRepository.Received(1).Delete(cart);
    }

    [Fact]
    public async Task ClearCartAsync_ShouldReturnCartResponse_WhenCartIsDeleted()
    {
        // Arrange
        var cartService = new CartService(_cartRepository);
        var cartId = new CartId(Guid.NewGuid());
        var cart = Cart.Create(cartId, Guid.NewGuid());

        // Configurando o mock para retornar o carrinho quando solicitado
        _cartRepository.GetByIdAsync(Arg.Is<CartId>(id => id == cartId)).Returns(cart);

        // Act
        var result = await cartService.ClearCartAsync(cartId.Value);

        // Assert
        result.Should().BeNull();
    }
}