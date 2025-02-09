using Postech.Fiap.CartsPayments.WebApi.Features.Orders.Entities;

namespace Postech.Fiap.CartsPayments.WebApi.UnitTests.Features.Orders.Entities;

public class OrderItemIdTests
{
    [Fact]
    public void OrderItemId_Should_Store_Guid_Value_Correctly()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var orderItemId = new OrderItemId(guid);

        // Assert
        orderItemId.Value.Should().Be(guid);
    }

    [Fact]
    public void New_Should_Generate_Unique_Guid()
    {
        // Act
        var orderItemId1 = OrderItemId.New();
        var orderItemId2 = OrderItemId.New();

        // Assert
        orderItemId1.Should().NotBe(orderItemId2);
        orderItemId1.Value.Should().NotBe(Guid.Empty);
        orderItemId2.Value.Should().NotBe(Guid.Empty);
    }
}