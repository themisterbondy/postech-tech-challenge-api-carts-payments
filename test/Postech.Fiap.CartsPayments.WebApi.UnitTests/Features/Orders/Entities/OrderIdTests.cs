using Postech.Fiap.CartsPayments.WebApi.Features.Orders.Entities;

namespace Postech.Fiap.CartsPayments.WebApi.UnitTests.Features.Orders.Entities;

public class OrderIdTests
{
    [Fact]
    public void OrderId_Should_Store_Guid_Value_Correctly()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var orderId = new OrderId(guid);

        // Assert
        orderId.Value.Should().Be(guid);
    }

    [Fact]
    public void New_Should_Generate_Unique_Guid()
    {
        // Act
        var orderId1 = OrderId.New();
        var orderId2 = OrderId.New();

        // Assert
        orderId1.Should().NotBe(orderId2);
        orderId1.Value.Should().NotBe(Guid.Empty);
        orderId2.Value.Should().NotBe(Guid.Empty);
    }
}