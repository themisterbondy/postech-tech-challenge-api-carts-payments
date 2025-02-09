namespace Postech.Fiap.CartsPayments.WebApi.Common.Messaging;

[ExcludeFromCodeCoverage]
public class QueueMessagePayload<T>
{
    public T MessageContent { get; init; }
    public MessageDetails MessageDetails { get; init; }
}

[ExcludeFromCodeCoverage]
public class MessageDetails
{
    public string MessageId { get; set; }
    public string PopReceipt { get; set; }
    public string MessageText { get; set; }
    public BinaryData Body { get; set; }
    public DateTimeOffset? NextVisibleOn { get; set; }
    public DateTimeOffset? InsertedOn { get; set; }
    public DateTimeOffset? ExpiresOn { get; set; }
    public long DequeueCount { get; set; }
}