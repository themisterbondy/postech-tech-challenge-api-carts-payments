using Postech.Fiap.CartsPayments.WebApi.Features.Orders.Entities;
using Postech.Fiap.CartsPayments.WebApi.Features.Products.Entities;
using Xunit.Abstractions;

namespace Postech.Fiap.CartsPayments.WebApi.UnitTests.Features.Orders.Entities;

public class OrderQueueTests(ITestOutputHelper output)
{
    [Fact]
    public void Create_Should_Return_Valid_OrderQueue()
    {
        // Arrange
        var orderId = OrderId.New();
        var customerId = Guid.NewGuid();
        var transactionId = Guid.NewGuid().ToString();
        var status = OrderQueueStatus.Received;
        var items = new List<OrderItem>
        {
            OrderItem.Create(
                new OrderItemId(Guid.NewGuid()), orderId, new ProductId(Guid.NewGuid()),
                "Pizza", 30.00m, 2, ProductCategory.Lanche
            )
        };

        // Act
        var orderQueue = OrderQueue.Create(orderId, customerId, items, transactionId, status);

        // Assert
        orderQueue.Id.Should().Be(orderId);
        orderQueue.CustomerId.Should().Be(customerId);
        orderQueue.TransactionId.Should().Be(transactionId);
        orderQueue.Status.Should().Be(status);
        orderQueue.Items.Should().BeEquivalentTo(items);
        orderQueue.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateStatus_Should_Change_Status_When_Valid()
    {
        // Arrange
        var orderQueue = OrderQueue.Create(
            OrderId.New(), Guid.NewGuid(), new List<OrderItem>(), Guid.NewGuid().ToString(),
            OrderQueueStatus.Received
        );

        // Act
        orderQueue.UpdateStatus(OrderQueueStatus.Preparing);

        // Assert
        orderQueue.Status.Should().Be(OrderQueueStatus.Preparing);
    }

    [Theory]
    [InlineData(OrderQueueStatus.Received, OrderQueueStatus.Preparing, true)]
    [InlineData(OrderQueueStatus.Preparing, OrderQueueStatus.Ready, true)]
    [InlineData(OrderQueueStatus.Ready, OrderQueueStatus.Completed, true)]
    [InlineData(OrderQueueStatus.Completed, OrderQueueStatus.Cancelled, false)]
    [InlineData(OrderQueueStatus.Ready, OrderQueueStatus.Preparing, false)]
    [InlineData(OrderQueueStatus.Completed, OrderQueueStatus.Ready, false)]
    [InlineData(OrderQueueStatus.Cancelled, OrderQueueStatus.Completed, false)]
    public void UpdateStatus_Should_Throw_Exception_When_Invalid_Transition(
        OrderQueueStatus currentStatus, OrderQueueStatus newStatus, bool isValidTransition)
    {
        // Arrange
        var orderQueue = OrderQueue.Create(
            OrderId.New(), Guid.NewGuid(), new List<OrderItem>(), Guid.NewGuid().ToString(), currentStatus
        );

        output.WriteLine($"Testing transition from {currentStatus} to {newStatus}");

        // Act
        var action = () => orderQueue.UpdateStatus(newStatus);

        // Assert
        if (isValidTransition)
        {
            action.Should().NotThrow();
            orderQueue.Status.Should().Be(newStatus);
        }
        else
        {
            action.Should().Throw<InvalidOperationException>()
                .WithMessage($"Cannot change status to {newStatus} when current status is {currentStatus}");
            orderQueue.Status.Should().Be(currentStatus);
        }
    }
}