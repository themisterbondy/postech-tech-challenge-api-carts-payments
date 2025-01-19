namespace Postech.Fiap.CartsPayments.WebApi.Features.Orders.Entities;

public class OrderQueue
{
    private OrderQueue(OrderId id, string? customerId, List<OrderItem> items, string transactionId,
        OrderQueueStatus status)
    {
        Id = id;
        CustomerId = customerId;
        Items = items;
        TransactionId = transactionId;
        Status = status;
    }

    private OrderQueue()
    {
    }

    public OrderId Id { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public OrderQueueStatus Status { get; set; }
    public string? CustomerId { get; set; }
    public List<OrderItem> Items { get; set; }

    public string? TransactionId { get; set; }

    public static OrderQueue Create(OrderId orderId, string? customerCpf, List<OrderItem> orderItems,
        string transactionId,
        OrderQueueStatus status)
    {
        return new OrderQueue(orderId, customerCpf, orderItems, transactionId, status);
    }

    public void UpdateStatus(OrderQueueStatus status)
    {
        if (status == OrderQueueStatus.Received && Status == OrderQueueStatus.Preparing)
            throw new InvalidOperationException("Cannot change status to Received when current status is Preparing");
        if (status == OrderQueueStatus.Preparing && Status == OrderQueueStatus.Ready)
            throw new InvalidOperationException("Cannot change status to Preparing when current status is Ready");
        if (status == OrderQueueStatus.Ready && Status == OrderQueueStatus.Completed)
            throw new InvalidOperationException("Cannot change status to Ready when current status is Completed");
        if (status == OrderQueueStatus.Completed && Status == OrderQueueStatus.Cancelled)
            throw new InvalidOperationException("Cannot change status to Completed when current status is Cancelled");
        Status = status;
    }
}