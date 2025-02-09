namespace Postech.Fiap.CartsPayments.WebApi.Features.Orders.Entities;

public record OrderId(Guid Value)
{
    public static OrderId New()
    {
        return new OrderId(Guid.NewGuid());
    }
}