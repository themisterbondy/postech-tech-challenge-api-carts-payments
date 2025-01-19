using Postech.Fiap.CartsPayments.WebApi.Features.Products.Entities;

namespace Postech.Fiap.CartsPayments.WebApi.Features.Orders.Contracts;

public class OrderDto
{
    public Guid OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; }
    public string? CustomerCpf { get; set; }
    public List<OrderItemDto> Items { get; set; }
    public string? TransactionId { get; set; }
}

public class OrderItemDto
{
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public decimal? UnitPrice { get; set; }
    public int Quantity { get; set; }
    public ProductCategory Category { get; set; } // Lanche, Acompanhamento, Bebida, Sobremesa
}