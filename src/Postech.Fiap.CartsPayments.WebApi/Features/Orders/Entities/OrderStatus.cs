namespace Postech.Fiap.CartsPayments.WebApi.Features.Orders.Entities;

public enum OrderQueueStatus
{
    Received,
    Preparing,
    Ready,
    Completed,
    Cancelled
}