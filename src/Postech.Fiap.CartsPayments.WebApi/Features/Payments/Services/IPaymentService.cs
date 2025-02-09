using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Contracts;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Notifications;

namespace Postech.Fiap.CartsPayments.WebApi.Features.Payments.Services;

public interface IPaymentService
{
    Task<Result<PaymentInitiationResponse>> InitiatePaymentAsync(Guid cartId, decimal amount);
    Task<Result<PaymentStatusResponse>> GetPaymentStatusAsync(Guid cartId);
    Task<Result> ProcessPaymentNotificationAsync(PaymentNotification notification, CancellationToken cancellationToken);
}