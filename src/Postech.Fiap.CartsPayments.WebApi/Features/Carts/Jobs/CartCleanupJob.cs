using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Repositories;
using Quartz;

namespace Postech.Fiap.CartsPayments.WebApi.Features.Carts.Jobs;

[ExcludeFromCodeCoverage]
public class CartCleanupJob(ICartRepository cartRepository, ILogger<CartCleanupJob> logger)
    : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("CartCleanupJob started.");

        var threshold = DateTime.UtcNow.AddMinutes(-15);
        await cartRepository.DeleteUnpaidCartsOlderThanAsync(threshold);

        logger.LogInformation("CartCleanupJob finished.");
    }
}