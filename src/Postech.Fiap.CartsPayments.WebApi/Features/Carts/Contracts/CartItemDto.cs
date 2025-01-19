using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Emun;

namespace Postech.Fiap.CartsPayments.WebApi.Features.Carts.Contracts;

public class CartRequest
{
    public string? CustomerId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}

public class CartItemDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}

public class CartResponse
{
    public Guid CartId { get; set; }
    public string CustomerId { get; set; }
    public List<CartItemDto> Items { get; set; }
    public decimal TotalAmount { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
}