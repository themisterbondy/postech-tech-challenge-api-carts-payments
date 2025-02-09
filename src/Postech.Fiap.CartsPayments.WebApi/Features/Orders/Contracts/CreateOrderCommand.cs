namespace Postech.Fiap.CartsPayments.WebApi.Features.Orders.Contracts;

public class CreateOrderCommand
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public string TransactionId { get; set; }
    public List<OrderItemDto> Items { get; set; } = [];
}