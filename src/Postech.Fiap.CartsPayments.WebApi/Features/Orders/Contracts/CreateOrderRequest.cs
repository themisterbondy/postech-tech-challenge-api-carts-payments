namespace Postech.Fiap.CartsPayments.WebApi.Features.Orders.Contracts;

public class EnqueueOrderRequest
{
    public string? CustomerCpf { get; set; }
    public List<OrderItemRequest> Items { get; set; }
}

public class OrderItemRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}