using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Entities;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Emun;
using Postech.Fiap.CartsPayments.WebApi.Features.Products.Entities;

namespace Postech.Fiap.CartsPayments.WebApi.UnitTests.Features.Carts.Entities;

public class CartTests
{
    [Fact]
    public void Create_ShouldInitializeCart_WhenGivenValidInputs()
    {
        // Arrange
        var cartId = CartId.New();
        var customerId = Guid.NewGuid();


        // Act
        var cart = Cart.Create(cartId, customerId);
        cart.TransactionId = "123";

        // Assert
        cart.Should().NotBeNull();
        cart.Id.Should().Be(cartId);
        cart.CustomerId.Should().Be(customerId);
        cart.CreatedAt.Should()
            .BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1)); // Verifica se a data de criação está correta
        cart.Items.Should().BeEmpty(); // O carrinho deve ser criado vazio
        cart.PaymentStatus.Should().Be(PaymentStatus.NotStarted);
        cart.TransactionId.Should().Be("123");
    }

    [Fact]
    public void AddItem_ShouldAddItemToCart_WhenValidItemProvided()
    {
        // Arrange
        var cartId = CartId.New();
        var cart = Cart.Create(cartId, Guid.NewGuid());
        var cartItem = CartItem.Create(CartItemId.New(), ProductId.New(), "Product 1", 10.99m, 1,
            ProductCategory.Lanche);

        // Act
        cart.AddItem(cartItem);

        // Assert
        cart.Items.Should().ContainSingle().Which.Should().Be(cartItem);
    }

    [Fact]
    public void RemoveItem_ShouldRemoveItemFromCart_WhenItemExists()
    {
        // Arrange
        var cartId = CartId.New();
        var cart = Cart.Create(cartId, Guid.NewGuid());
        var cartItem = CartItem.Create(CartItemId.New(), ProductId.New(), "Product 1", 10.99m, 1,
            ProductCategory.Lanche);
        cart.AddItem(cartItem);

        // Act
        cart.RemoveItem(cartItem.Id);

        // Assert
        cart.Items.Should().BeEmpty();
    }

    [Fact]
    public void RemoveItem_ShouldDoNothing_WhenItemDoesNotExist()
    {
        // Arrange
        var cartId = CartId.New();
        var cart = Cart.Create(cartId, Guid.NewGuid());
        var itemId = CartItemId.New();

        // Act
        cart.RemoveItem(itemId);

        // Assert
        cart.Items.Should().BeEmpty(); // Carrinho permanece vazio
    }

    [Fact]
    public void UpdatePaymentStatus_ShouldChangePaymentStatus()
    {
        // Arrange
        var cartId = CartId.New();
        var cart = Cart.Create(cartId, Guid.NewGuid());

        // Act
        cart.UpdatePaymentStatus(PaymentStatus.Accepted);

        // Assert
        cart.PaymentStatus.Should().Be(PaymentStatus.Accepted);
    }

    [Fact]
    public void Items_ShouldInitializeAsEmptyList()
    {
        // Arrange
        var cart = Cart.Create(CartId.New(), Guid.NewGuid());

        // Act
        var items = cart.Items;

        // Assert
        Assert.NotNull(items); // Garante que a lista foi inicializada
        Assert.Empty(items); // Garante que a lista está vazia por padrão
    }

    [Fact]
    public void Items_ShouldBeReadOnlyAfterInitialization()
    {
        // Arrange
        var cart = Cart.Create(CartId.New(), Guid.NewGuid());
        var cartIten = CartItem.Create(CartItemId.New(), ProductId.New(), "Item 1", 10.99m, 1,
            ProductCategory.Lanche);

        cart.AddItem(cartIten);

        // Act
        var items = cart.Items;

        // Assert
        Assert.Single(items); // Garante que a lista contém o item inicial
        Assert.Equal("Item 1", items[0].ProductName);
    }

    [Fact]
    public void ShouldAllowAddingItemsToExistingList()
    {
        // Arrange
        var cart = Cart.Create(CartId.New(), Guid.NewGuid());
        var cartIten = CartItem.Create(CartItemId.New(), ProductId.New(), "Item A", 10.99m, 2,
            ProductCategory.Lanche);

        // Act
        cart.Items.Add(cartIten);

        // Assert
        Assert.NotEmpty(cart.Items); // Garante que a lista recebeu o item
        Assert.Equal("Item A", cart.Items[0].ProductName);
        Assert.Equal(2, cart.Items[0].Quantity);
    }
}