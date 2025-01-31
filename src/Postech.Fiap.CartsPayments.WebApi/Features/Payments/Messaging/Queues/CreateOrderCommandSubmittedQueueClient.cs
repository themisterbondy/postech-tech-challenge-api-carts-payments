using Microsoft.Extensions.Options;
using Postech.Fiap.CartsPayments.WebApi.Common.Messaging;
using Postech.Fiap.CartsPayments.WebApi.Settings;

namespace Postech.Fiap.CartsPayments.WebApi.Features.Payments.Messaging.Queues;

[ExcludeFromCodeCoverage]
public class CreateOrderCommandSubmittedQueueClient(IQueue queue, IOptions<AzureQueueSettings> azureQueueSettings)
    : ICreateOrderCommandSubmittedQueueClient
{
    private readonly AzureQueueSettings settings = azureQueueSettings.Value;
    private string QueueName => settings.CreateOrderCommand;

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken)
    {
        await queue.PublishMessageAsync(QueueName, message, cancellationToken);
    }

    public async Task<Result<QueueMessagePayload<T>>> ReceiveAsync<T>(CancellationToken cancellationToken)
    {
        return await queue.ReceiveMessageAsync<T>(QueueName, cancellationToken);
    }

    public async Task<Result<List<QueueMessagePayload<T>>>> ReceiveAllAsync<T>(CancellationToken cancellationToken)
    {
        return await queue.ReceiveMessagesAsync<T>(QueueName, 32, cancellationToken);
    }

    public async Task<Result<QueueMessagePayload<T>>> UpdateAsync<T>(T message, string messageId, string popReceipt,
        CancellationToken cancellationToken)
    {
        return await queue.UpdateMessageAsync(QueueName, message, messageId, popReceipt, cancellationToken);
    }

    public async Task<Result> DeleteAsync(string messageId, string popReceipt, CancellationToken cancellationToken)
    {
        return await queue.DeleteMessageAsync(QueueName, messageId, popReceipt, cancellationToken);
    }
}