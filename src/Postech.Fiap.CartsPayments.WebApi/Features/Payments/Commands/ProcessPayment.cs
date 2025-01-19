using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Contracts;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Emun;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Notifications;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Services;

namespace Postech.Fiap.CartsPayments.WebApi.Features.Payments.Commands;

public class ProcessPayment
{
    public class Command : IRequest<Result<PaymentStatusResponse>>
    {
        public string TransactionId { get; set; }
        public PaymentStatus Status { get; set; }
        public decimal Amount { get; set; }
    }


    public class Handler(IPaymentService paymentService) : IRequestHandler<Command, Result<PaymentStatusResponse>>
    {
        public async Task<Result<PaymentStatusResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            var notification = new PaymentNotification
            {
                TransactionId = request.TransactionId,
                Status = request.Status,
                Amount = request.Amount
            };

            var result = await paymentService.ProcessPaymentNotificationAsync(notification, cancellationToken);

            return result.IsSuccess
                ? Result.Success(new PaymentStatusResponse
                {
                    TransactionId = request.TransactionId,
                    Status = request.Status
                })
                : Result.Failure<PaymentStatusResponse>(result.Error);
        }
    }
}