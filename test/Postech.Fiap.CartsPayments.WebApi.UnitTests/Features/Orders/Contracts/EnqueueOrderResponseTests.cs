using Postech.Fiap.CartsPayments.WebApi.Features.Orders.Contracts;
using Postech.Fiap.CartsPayments.WebApi.Features.Orders.Entities;
using Postech.Fiap.CartsPayments.WebApi.Features.Products.Entities;

namespace Postech.Fiap.CartsPayments.WebApi.UnitTests.Features.Orders.Contracts;

public class EnqueueOrderResponseTests
{
    [Fact]
    public void EnqueueOrderResponse_Should_Set_And_Get_Properties_Correctly()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var customerCpf = "123.456.789-00";
        var status = OrderQueueStatus.Ready;
        var transactionId = Guid.NewGuid().ToString();
        var items = new List<OrderItemDto>
        {
            new()
            {
                ProductId = Guid.NewGuid(), ProductName = "Burger", UnitPrice = 15.00m, Quantity = 2,
                Category = ProductCategory.Lanche
            },
            new()
            {
                ProductId = Guid.NewGuid(), ProductName = "Soda", UnitPrice = 5.00m, Quantity = 1,
                Category = ProductCategory.Bebida
            }
        };

        // Act
        var response = new EnqueueOrderResponse
        {
            OrderId = orderId,
            CreatedAt = createdAt,
            CustomerCpf = customerCpf,
            Status = status,
            Items = items,
            TransactionId = transactionId
        };

        // Assert
        response.OrderId.Should().Be(orderId);
        response.CreatedAt.Should().BeCloseTo(createdAt, TimeSpan.FromSeconds(1));
        response.CustomerCpf.Should().Be(customerCpf);
        response.Status.Should().Be(status);
        response.Items.Should().BeEquivalentTo(items);
        response.TransactionId.Should().Be(transactionId);
    }

    [Fact]
    public void EnqueueOrderResponse_Should_Allow_Null_TransactionId()
    {
        // Arrange & Act
        var response = new EnqueueOrderResponse
        {
            OrderId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            CustomerCpf = "123.456.789-00",
            Status = OrderQueueStatus.Ready,
            Items = new List<OrderItemDto>(),
            TransactionId = null
        };

        // Assert
        response.TransactionId.Should().BeNull();
    }
}

public class ListOrdersResponseTests
{
    [Fact]
    public void ListOrdersResponse_Should_Set_And_Get_Properties_Correctly()
    {
        // Arrange
        var orders = new List<OrderDto>
        {
            new()
            {
                OrderId = Guid.NewGuid(), OrderDate = DateTime.UtcNow, Status = "Completed",
                CustomerCpf = "123.456.789-00", Items = new List<OrderItemDto>(),
                TransactionId = Guid.NewGuid().ToString()
            },
            new()
            {
                OrderId = Guid.NewGuid(), OrderDate = DateTime.UtcNow, Status = "Pending",
                CustomerCpf = "987.654.321-00", Items = new List<OrderItemDto>(), TransactionId = null
            }
        };

        // Act
        var response = new ListOrdersResponse
        {
            Orders = orders
        };

        // Assert
        response.Orders.Should().BeEquivalentTo(orders);
    }

    [Fact]
    public void ListOrdersResponse_Should_Allow_Empty_Order_List()
    {
        // Act
        var response = new ListOrdersResponse
        {
            Orders = new List<OrderDto>() // Lista vazia
        };

        // Assert
        response.Orders.Should().NotBeNull().And.BeEmpty();
    }
}