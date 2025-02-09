using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Emun;

namespace Postech.Fiap.CartsPayments.WebApi.Features.Payments.Notifications;

public class PaymentNotification
{
    public string TransactionId { get; set; }
    public PaymentStatus Status { get; set; }
    public decimal Amount { get; set; }
}