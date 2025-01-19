using Postech.Fiap.CartsPayments.WebApi.Features.Orders.Entities;

namespace Postech.Fiap.CartsPayments.WebApi.Features.Orders.Repositories;

public interface IOrderQueueRepository
{
    Task<OrderQueue?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(OrderQueue orderQueue, CancellationToken cancellationToken);
    Task UpdateStatusAsync(Guid id, OrderQueueStatus status, CancellationToken cancellationToken);
    Task CancelOrdersNotPreparingWithinAsync(DateTime threshold);
    Task<OrderQueue?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken);
}