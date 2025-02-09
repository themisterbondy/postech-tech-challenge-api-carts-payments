using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Entities;
using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Repositories;
using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Services;
using Postech.Fiap.CartsPayments.WebApi.Features.Orders.Contracts;
using Postech.Fiap.CartsPayments.WebApi.Features.Orders.Entities;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Contracts;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Emun;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Messaging.Queues;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Notifications;

namespace Postech.Fiap.CartsPayments.WebApi.Features.Payments.Services;

public class PaymentService(
    ICartRepository cartRepository,
    ICreateOrderCommandSubmittedQueueClient createOrderCommandSubmittedQueueClient,
    ICartService cartService) : IPaymentService
{
    public async Task<Result<PaymentInitiationResponse>> InitiatePaymentAsync(Guid cartId, decimal amount)
    {
        var transactionId = Guid.NewGuid().ToString();
        return await Task.FromResult(new PaymentInitiationResponse
        {
            TransactionId = transactionId,
            QrCodeImageUrl = $"https://payment.com/qrcode/{transactionId}.png"
        });
    }

    public async Task<Result<PaymentStatusResponse>> GetPaymentStatusAsync(Guid cartId)
    {
        var cart = await cartRepository.GetByIdAsync(new CartId(cartId));
        if (cart == null)
            return Result.Failure<PaymentStatusResponse>(Error.Failure("PaymentService.GetPaymentStatusAsync",
                "Cart not found"));

        return new PaymentStatusResponse
        {
            CartId = cart.Id.Value,
            Status = cart.PaymentStatus,
            TransactionId = cart.TransactionId
        };
    }

    public async Task<Result> ProcessPaymentNotificationAsync(PaymentNotification notification,
        CancellationToken cancellationToken)
    {
        // 1. Buscar o carrinho pelo TransactionId
        var cart = await cartRepository.GetByTransactionIdAsync(notification.TransactionId);
        if (cart == null)
            return Result.Failure(Error.Failure("PaymentService.ProcessPaymentNotificationAsync",
                "Cart not found or already processed"));

        if (notification.Status == PaymentStatus.Accepted)
        {
            var orderId = OrderId.New();

            var createOrderCommand = new CreateOrderCommand
            {
                OrderId = orderId.Value,
                CustomerId = cart.CustomerId,
                TransactionId = notification.TransactionId,
                Items = cart.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId.Value,
                    ProductName = i.ProductName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    Category = i.Category
                }).ToList()
            };

            createOrderCommandSubmittedQueueClient.PublishAsync(createOrderCommand, cancellationToken);

            await cartService.ClearCartAsync(cart.Id.Value);
        }
        else if (notification.Status == PaymentStatus.Rejected)
        {
            cart.UpdatePaymentStatus(PaymentStatus.Rejected);
            await cartRepository.UpdateStatusAsync(cart);
        }

        return Result.Success();
    }
}