using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Emun;

namespace Postech.Fiap.CartsPayments.WebApi.Features.Carts.Entities;

public class Cart
{
    private Cart(CartId id, Guid customerId)
    {
        Id = id;
        CustomerId = customerId;
        CreatedAt = DateTime.UtcNow;
        PaymentStatus = PaymentStatus.NotStarted;
    }

    public CartId Id { get; init; }
    public Guid CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<CartItem> Items { get; } = [];
    public PaymentStatus PaymentStatus { get; set; }
    public string? TransactionId { get; set; }

    public static Cart Create(CartId id, Guid customerId)
    {
        return new Cart(id, customerId);
    }

    public void AddItem(CartItem item)
    {
        Items.Add(item);
    }

    public void RemoveItem(CartItemId itemId)
    {
        Items.RemoveAll(x => x.Id == itemId);
    }

    public void UpdatePaymentStatus(PaymentStatus status)
    {
        PaymentStatus = status;
    }
}