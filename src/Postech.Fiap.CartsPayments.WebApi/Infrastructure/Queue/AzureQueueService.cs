using Azure.Storage.Queues;
using Newtonsoft.Json;
using Postech.Fiap.CartsPayments.WebApi.Common.Messaging;

namespace Postech.Fiap.CartsPayments.WebApi.Infrastructure.Queue;

[ExcludeFromCodeCoverage]
public class AzureQueueService(QueueServiceClient queueServiceClient) : IQueue
{
    public async Task<Result> PublishMessageAsync<T>(string queueName, T message,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var serializedMessage = JsonConvert.SerializeObject(message);
            var queueClient = queueServiceClient.GetQueueClient(queueName);
            await queueClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
            await queueClient.SendMessageAsync(serializedMessage, cancellationToken);

            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Failure(Error.Failure("AzureQueueService.PublishMessageAsync",
                "Error sending message to queue: " + e.Message));
        }
    }

    public async Task<Result<QueueMessagePayload<T>>> ReceiveMessageAsync<T>(string queueName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var queueClient = queueServiceClient.GetQueueClient(queueName);
            await queueClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
            var response = await queueClient.ReceiveMessageAsync(TimeSpan.FromSeconds(30), cancellationToken);
            if (response.Value != null)
            {
                var messageDetails = response.Value;

                var deserializedMessage = JsonConvert.DeserializeObject<T>(messageDetails.MessageText);
                return Result.Success(new QueueMessagePayload<T>
                {
                    MessageContent = deserializedMessage,
                    MessageDetails = new MessageDetails
                    {
                        MessageId = messageDetails.MessageId,
                        PopReceipt = messageDetails.PopReceipt,
                        MessageText = messageDetails.MessageText,
                        Body = messageDetails.Body,
                        NextVisibleOn = messageDetails.NextVisibleOn,
                        InsertedOn = messageDetails.InsertedOn,
                        ExpiresOn = messageDetails.ExpiresOn,
                        DequeueCount = messageDetails.DequeueCount
                    }
                });
            }
        }
        catch (Exception e)
        {
            return Result.Failure<QueueMessagePayload<T>>(Error.Failure("AzureQueueService.ReceiveMessageAsync",
                "Error receiving message from queue: " + e.Message));
        }

        return Result.Success(default(QueueMessagePayload<T>));
    }

    public async Task<Result<List<QueueMessagePayload<T>>>> ReceiveMessagesAsync<T>(string queueName, int maxMessages,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var queueClient = queueServiceClient.GetQueueClient(queueName);
            await queueClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
            var messages = new List<QueueMessagePayload<T>>();

            var response =
                await queueClient.ReceiveMessagesAsync(maxMessages, TimeSpan.FromSeconds(5), cancellationToken);
            if (response.Value != null)
            {
                var messageDetails = response.Value;

                messages.AddRange(from messageDetail in messageDetails
                    let deserializedMessage = JsonConvert.DeserializeObject<T>(messageDetail.MessageText)
                    select new QueueMessagePayload<T>
                    {
                        MessageContent = deserializedMessage,
                        MessageDetails = new MessageDetails
                        {
                            MessageId = messageDetail.MessageId,
                            PopReceipt = messageDetail.PopReceipt,
                            MessageText = messageDetail.MessageText,
                            Body = messageDetail.Body,
                            NextVisibleOn = messageDetail.NextVisibleOn,
                            InsertedOn = messageDetail.InsertedOn,
                            ExpiresOn = messageDetail.ExpiresOn,
                            DequeueCount = messageDetail.DequeueCount
                        }
                    });

                return Result.Success(messages);
            }
        }
        catch (Exception e)
        {
            return Result.Failure<List<QueueMessagePayload<T>>>(Error.Failure("AzureQueueService.ReceiveMessageAsync",
                "Error receiving message from queue: " + e.Message));
        }

        return Result.Success(default(List<QueueMessagePayload<T>>));
    }

    public async Task<Result<QueueMessagePayload<T>>> UpdateMessageAsync<T>(string queueName, T message,
        string messageId,
        string popReceipt, CancellationToken cancellationToken = default)
    {
        try
        {
            var serializedMessage = JsonConvert.SerializeObject(message);
            var queueClient = queueServiceClient.GetQueueClient(queueName);
            var result = await queueClient.UpdateMessageAsync(messageId, popReceipt, serializedMessage,
                TimeSpan.FromSeconds(60), cancellationToken);

            return Result.Success(new QueueMessagePayload<T>
            {
                MessageContent = message,
                MessageDetails = new MessageDetails
                {
                    PopReceipt = result.Value.PopReceipt,
                    MessageId = messageId
                }
            });
        }
        catch (Exception e)
        {
            return Result.Failure<QueueMessagePayload<T>>(Error.Failure("AzureQueueService.UpdateMessageAsync",
                "Error updating message in queue: " + e.Message));
        }
    }

    public async Task<Result> DeleteMessageAsync(string queueName, string messageId, string popReceipt,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var queueClient = queueServiceClient.GetQueueClient(queueName);
            var respose = await queueClient.DeleteMessageAsync(messageId, popReceipt, cancellationToken);

            if (respose.IsError)
                return Result.Failure(Error.Failure("AzureQueueService.DeleteMessageAsync",
                    "Error deleting message from queue: " + respose.Status));

            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Failure(Error.Failure("AzureQueueService.DeleteMessageAsync",
                "Error deleting message from queue: " + e.Message));
        }
    }
}