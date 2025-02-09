using Postech.Fiap.CartsPayments.WebApi.Features.Orders.Contracts;

namespace Postech.Fiap.CartsPayments.WebApi.UnitTests.Features.Orders.Contracts;

public class EnqueueOrderRequestTests
{
    [Fact]
    public void EnqueueOrderRequest_Should_Set_And_Get_Properties_Correctly()
    {
        // Arrange
        var customerCpf = "123.456.789-00";
        var items = new List<OrderItemRequest>
        {
            new() { ProductId = Guid.NewGuid(), Quantity = 2 },
            new() { ProductId = Guid.NewGuid(), Quantity = 1 }
        };

        // Act
        var request = new EnqueueOrderRequest
        {
            CustomerCpf = customerCpf,
            Items = items
        };

        // Assert
        request.CustomerCpf.Should().Be(customerCpf);
        request.Items.Should().BeEquivalentTo(items);
    }

    [Fact]
    public void EnqueueOrderRequest_Should_Allow_Null_CustomerCpf()
    {
        // Arrange & Act
        var request = new EnqueueOrderRequest
        {
            CustomerCpf = null,
            Items = new List<OrderItemRequest>()
        };

        // Assert
        request.CustomerCpf.Should().BeNull();
    }

    [Fact]
    public void EnqueueOrderRequest_Should_Allow_Empty_Items_List()
    {
        // Act
        var request = new EnqueueOrderRequest
        {
            CustomerCpf = "123.456.789-00",
            Items = new List<OrderItemRequest>() // Lista vazia
        };

        // Assert
        request.Items.Should().NotBeNull().And.BeEmpty();
    }
}

public class OrderItemRequestTests
{
    [Fact]
    public void OrderItemRequest_Should_Set_And_Get_Properties_Correctly()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var quantity = 3;

        // Act
        var itemRequest = new OrderItemRequest
        {
            ProductId = productId,
            Quantity = quantity
        };

        // Assert
        itemRequest.ProductId.Should().Be(productId);
        itemRequest.Quantity.Should().Be(quantity);
    }
}