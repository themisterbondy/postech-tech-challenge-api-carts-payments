using Postech.Fiap.CartsPayments.WebApi.Features.Orders.Contracts;
using Postech.Fiap.CartsPayments.WebApi.Features.Products.Entities;

namespace Postech.Fiap.CartsPayments.WebApi.UnitTests.Features.Orders.Contracts;

public class CreateOrderCommandTests
{
    [Fact]
    public void CreateOrderCommand_Should_Set_And_Get_Properties_Correctly()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var transactionId = Guid.NewGuid().ToString();
        var items = new List<OrderItemDto>
        {
            new()
            {
                ProductId = Guid.NewGuid(), ProductName = "Pizza", UnitPrice = 30.00m, Quantity = 1,
                Category = ProductCategory.Lanche
            },
            new()
            {
                ProductId = Guid.NewGuid(), ProductName = "Soda", UnitPrice = 5.00m, Quantity = 2,
                Category = ProductCategory.Bebida
            }
        };

        // Act
        var command = new CreateOrderCommand
        {
            OrderId = orderId,
            CustomerId = customerId,
            TransactionId = transactionId,
            Items = items
        };

        // Assert
        command.OrderId.Should().Be(orderId);
        command.CustomerId.Should().Be(customerId);
        command.TransactionId.Should().Be(transactionId);
        command.Items.Should().BeEquivalentTo(items);
    }

    [Fact]
    public void CreateOrderCommand_Should_Initialize_Empty_List_By_Default()
    {
        // Act
        var command = new CreateOrderCommand();

        // Assert
        command.Items.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void CreateOrderCommand_Should_Allow_Empty_TransactionId()
    {
        // Arrange & Act
        var command = new CreateOrderCommand
        {
            OrderId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            TransactionId = string.Empty, // Simulando um TransactionId vazio
            Items = new List<OrderItemDto>()
        };

        // Assert
        command.TransactionId.Should().BeEmpty();
    }
}