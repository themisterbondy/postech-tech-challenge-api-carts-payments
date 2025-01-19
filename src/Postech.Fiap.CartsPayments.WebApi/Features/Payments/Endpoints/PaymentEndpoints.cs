using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Commands;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Contracts;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Notifications;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Querys;

namespace Postech.Fiap.CartsPayments.WebApi.Features.Payments.Endpoints;

public class PaymentEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/payments/webhook", async (ISender sender, PaymentNotification notification) =>
            {
                var result = await sender.Send(new ProcessPayment.Command
                {
                    TransactionId = notification.TransactionId,
                    Status = notification.Status,
                    Amount = notification.Amount
                });

                return result.IsSuccess
                    ? Results.NoContent()
                    : result.ToProblemDetails();
            })
            .WithName("PaymentWebhook")
            .WithTags("Payments")
            .Accepts<PaymentNotification>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .WithOpenApi();

        //Consultar status pagamento pedido, que informa se o pagamento foi aprovado ou não.
        app.MapGet("/api/payments/{cartId}/status", async (ISender sender, Guid cartId) =>
            {
                var result = await sender.Send(new GetPaymentStatus.Query { CartId = cartId });
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : result.ToProblemDetails();
            })
            .WithName("GetPaymentStatus")
            .WithTags("Payments")
            .Produces<PaymentStatusResponse>(StatusCodes.Status200OK)
            .WithOpenApi();
    }
}