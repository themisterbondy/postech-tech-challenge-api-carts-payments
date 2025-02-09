namespace Postech.Fiap.CartsPayments.WebApi.Features.Payments.Contracts;

public class PaymentInitiationResponse
{
    public string TransactionId { get; set; }
    public string QrCodeImageUrl { get; set; }
}