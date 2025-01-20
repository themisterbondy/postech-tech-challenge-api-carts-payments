namespace Postech.Fiap.CartsPayments.WebApi.Features.Payments.Contracts;

public class PaymentRejectedEvent
{
    public string TransactionId { get; set; }
    public Guid cartId { get; set; }
    public Guid customerId { get; set; }
    public DateTime CreatedAt { get; set; }

}