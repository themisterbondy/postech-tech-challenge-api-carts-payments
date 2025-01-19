namespace Postech.Fiap.CartsPayments.WebApi.Features.Orders.Contracts;

public class CheckoutResponse
{
    public Guid CartId { get; set; }
    public string CustomerId { get; set; }
    public string Status { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItemDto> Items { get; set; }
    public string QrCodeImageUrl { get; set; }
    public string TransactionId { get; set; }
}