namespace Postech.Fiap.CartsPayments.WebApi.Features.Payments.Emun;

public enum PaymentStatus
{
    NotStarted, // Pagamento ainda não iniciado
    Pending, // Pagamento iniciado, aguardando confirmação
    Accepted, // Pagamento confirmado
    Rejected // Pagamento rejeitado
}