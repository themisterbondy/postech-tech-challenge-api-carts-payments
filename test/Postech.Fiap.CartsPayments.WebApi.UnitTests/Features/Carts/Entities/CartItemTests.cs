using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Entities;
using Postech.Fiap.CartsPayments.WebApi.Features.Products.Entities;

namespace Postech.Fiap.CartsPayments.WebApi.UnitTests.Features.Carts.Entities;

public class CartItemTests
{
    [Fact]
    public void Create_ShouldReturnCartItem_WhenGivenValidInputs()
    {
        // Arrange
        var cartItemId = CartItemId.New();
        var productId = ProductId.New();
        const string productName = "Test Product";
        const decimal unitPrice = 10.50m;
        const int quantity = 5;
        const ProductCategory category = ProductCategory.Lanche; // Categoria válida

        // Act
        var result = CartItem.Create(cartItemId, productId, productName, unitPrice, quantity, category);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(cartItemId);
        result.ProductId.Should().Be(productId);
        result.ProductName.Should().Be(productName);
        result.UnitPrice.Should().Be(unitPrice);
        result.Quantity.Should().Be(quantity);
        result.Category.Should().Be(ProductCategory.Lanche); // Categoria deve ser Lanche
        result.CartId.Should().BeNull();
    }

    [Fact]
    public void Create_ShouldAssignCorrectCategory_WhenGivenDifferentCategories()
    {
        // Arrange
        var cartItemId = CartItemId.New();
        var productId = ProductId.New();
        const string productName = "Test Product";
        const decimal unitPrice = 10.50m;
        const int quantity = 5;

        var lancheItem =
            CartItem.Create(cartItemId, productId, productName, unitPrice, quantity, ProductCategory.Lanche);
        lancheItem.Category.Should().Be(ProductCategory.Lanche);

        var acompanhamentoItem = CartItem.Create(cartItemId, productId, productName, unitPrice, quantity,
            ProductCategory.Acompanhamento);
        acompanhamentoItem.Category.Should().Be(ProductCategory.Acompanhamento);

        var bebidaItem =
            CartItem.Create(cartItemId, productId, productName, unitPrice, quantity, ProductCategory.Bebida);
        bebidaItem.Category.Should().Be(ProductCategory.Bebida);

        var sobremesaItem = CartItem.Create(cartItemId, productId, productName, unitPrice, quantity,
            ProductCategory.Sobremesa);
        sobremesaItem.Category.Should().Be(ProductCategory.Sobremesa);
    }
}