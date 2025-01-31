namespace Postech.Fiap.CartsPayments.WebApi.Features.Orders.Entities;

public class OrderQueue
{
    private OrderQueue(OrderId id, Guid customerId, List<OrderItem> items, string transactionId,
        OrderQueueStatus status)
    {
        Id = id;
        CustomerId = customerId;
        Items = items;
        TransactionId = transactionId;
        Status = status;
    }


    public OrderId Id { get; set; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public OrderQueueStatus Status { get; set; }
    public Guid CustomerId { get; set; }
    public List<OrderItem> Items { get; set; }

    public string? TransactionId { get; set; }

    public static OrderQueue Create(OrderId orderId, Guid customerCpf, List<OrderItem> orderItems,
        string transactionId,
        OrderQueueStatus status)
    {
        return new OrderQueue(orderId, customerCpf, orderItems, transactionId, status);
    }

    public void UpdateStatus(OrderQueueStatus newStatus)
    {
        Console.WriteLine($"Changing from {Status} to {newStatus}");

        if (newStatus == OrderQueueStatus.Received && Status == OrderQueueStatus.Preparing)
            throw new InvalidOperationException("Cannot change status to Received when current status is Preparing");
        if (newStatus == OrderQueueStatus.Preparing && Status == OrderQueueStatus.Ready)
            throw new InvalidOperationException("Cannot change status to Preparing when current status is Ready");
        if (newStatus == OrderQueueStatus.Ready && Status == OrderQueueStatus.Completed)
            throw new InvalidOperationException("Cannot change status to Ready when current status is Completed");
        if (Status == OrderQueueStatus.Completed && newStatus == OrderQueueStatus.Cancelled)
            throw new InvalidOperationException("Cannot change status to Cancelled when current status is Completed");
        if (Status == OrderQueueStatus.Cancelled && newStatus == OrderQueueStatus.Completed)
            throw new InvalidOperationException("Cannot change status to Completed when current status is Cancelled");

        Status = newStatus;
    }
}