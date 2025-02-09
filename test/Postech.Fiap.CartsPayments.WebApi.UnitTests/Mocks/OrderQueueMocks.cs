using Bogus;
using Postech.Fiap.CartsPayments.WebApi.Features.Orders.Entities;

namespace Postech.Fiap.CartsPayments.WebApi.UnitTests.Mocks;

public static class OrderQueueMocks
{
    public static OrderQueue GenerateValidOrderQueue()
    {
        var faker = new Faker();
        var orderId = new OrderId(faker.Random.Guid());
        var customerCpf = faker.Random.Guid();
        var transactionId = faker.Random.Guid().ToString();
        var items = new List<OrderItem> { OrderItemMocks.GenerateValidOrderItem() };

        return OrderQueue.Create(orderId, customerCpf, items, transactionId, OrderQueueStatus.Received);
    }

    public static OrderQueue GenerateInvalidOrderQueue()
    {
        return OrderQueue.Create(null, Guid.Empty, null, null, OrderQueueStatus.Received);
    }
}