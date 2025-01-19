using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Emun;
using Postech.Fiap.CartsPayments.WebApi.Features.Products.Entities;

namespace Postech.Fiap.CartsPayments.WebApi.Features.Carts.Contracts;

public class CartRequest
{
    public Guid CustomerId { get; set; }
    public List<CartItemRequest> Items { get; set; }
}

public class CartItemRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}

public class CartItemDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }

    public ProductCategory Category { get; set; }
}

public class CartResponse
{
    public Guid CartId { get; set; }
    public Guid CustomerId { get; set; }
    public List<CartItemDto> Items { get; set; }
    public decimal TotalAmount { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
}