using Microsoft.EntityFrameworkCore;
using Postech.Fiap.CartsPayments.WebApi.Features.Orders.Entities;
using Postech.Fiap.CartsPayments.WebApi.Persistence;

namespace Postech.Fiap.CartsPayments.WebApi.Features.Orders.Repositories;

public class OrderQueueRepository(ApplicationDbContext context) : IOrderQueueRepository
{
    public async Task<OrderQueue?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.OrderQueue
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == new OrderId(id), cancellationToken);
    }

    public async Task AddAsync(OrderQueue orderQueue, CancellationToken cancellationToken)
    {
        await context.OrderQueue.AddAsync(orderQueue, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateStatusAsync(Guid id, OrderQueueStatus status, CancellationToken cancellationToken)
    {
        var orderQueue = await GetByIdAsync(id, cancellationToken);
        if (orderQueue != null)
        {
            orderQueue.Status = status;
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task CancelOrdersNotPreparingWithinAsync(DateTime threshold)
    {
        var ordersToCancel = await context.OrderQueue
            .Where(o => o.CreatedAt < threshold && o.Status == OrderQueueStatus.Received)
            .ToListAsync();

        foreach (var order in ordersToCancel) order.Status = OrderQueueStatus.Cancelled;

        context.OrderQueue.UpdateRange(ordersToCancel);
        await context.SaveChangesAsync();
    }

    public Task<OrderQueue?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken)
    {
        return context.OrderQueue
            .AsNoTracking()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.TransactionId == transactionId, cancellationToken: cancellationToken);
    }
}