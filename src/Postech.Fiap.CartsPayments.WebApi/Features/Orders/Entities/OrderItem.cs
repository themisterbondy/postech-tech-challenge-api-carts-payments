
using Postech.Fiap.CartsPayments.WebApi.Features.Products.Entities;

namespace Postech.Fiap.CartsPayments.WebApi.Features.Orders.Entities;

public class OrderItem
{
    private OrderItem(OrderItemId id, OrderId orderId, ProductId productId, string productName,
        decimal? unitPrice, int quantity, ProductCategory category)
    {
        Id = id;
        OrderId = orderId;
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
        Category = category;
    }

    private OrderItem()
    {
    }

    public OrderItemId Id { get; set; }
    public OrderId OrderId { get; set; }
    public ProductId ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal? UnitPrice { get; set; }
    public int Quantity { get; set; }
    public ProductCategory Category { get; set; }

    public static OrderItem Create(OrderItemId id, OrderId orderId, ProductId productId, string productName,
        decimal? unitPrice, int quantity, ProductCategory category)
    {
        return new OrderItem(id, orderId, productId, productName, unitPrice, quantity, category);
    }
}