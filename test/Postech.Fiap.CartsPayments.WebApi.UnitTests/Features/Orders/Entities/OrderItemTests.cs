using Postech.Fiap.CartsPayments.WebApi.Features.Orders.Entities;
using Postech.Fiap.CartsPayments.WebApi.Features.Products.Entities;

namespace Postech.Fiap.CartsPayments.WebApi.UnitTests.Features.Orders.Entities;

public class OrderItemTests
{
    [Fact]
    public void Create_Should_Return_Valid_OrderItem()
    {
        // Arrange
        var id = new OrderItemId(Guid.NewGuid());
        var orderId = OrderId.New();
        var productId = new ProductId(Guid.NewGuid());
        var productName = "Pizza";
        var unitPrice = 30.00m;
        var quantity = 2;
        var category = ProductCategory.Lanche;

        // Act
        var orderItem = OrderItem.Create(id, orderId, productId, productName, unitPrice, quantity, category);

        // Assert
        orderItem.Id.Should().Be(id);
        orderItem.OrderId.Should().Be(orderId);
        orderItem.ProductId.Should().Be(productId);
        orderItem.ProductName.Should().Be(productName);
        orderItem.UnitPrice.Should().Be(unitPrice);
        orderItem.Quantity.Should().Be(quantity);
        orderItem.Category.Should().Be(category);
    }

    [Fact]
    public void Create_Should_Allow_Null_UnitPrice()
    {
        // Arrange
        var id = new OrderItemId(Guid.NewGuid());
        var orderId = OrderId.New();
        var productId = new ProductId(Guid.NewGuid());
        var productName = "Soda";
        decimal? unitPrice = null;
        var quantity = 1;
        var category = ProductCategory.Bebida;

        // Act
        var orderItem = OrderItem.Create(id, orderId, productId, productName, unitPrice, quantity, category);

        // Assert
        orderItem.UnitPrice.Should().BeNull();
    }
}