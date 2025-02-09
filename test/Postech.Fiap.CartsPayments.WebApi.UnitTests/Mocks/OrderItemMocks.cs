using Bogus;
using Postech.Fiap.CartsPayments.WebApi.Features.Orders.Entities;
using Postech.Fiap.CartsPayments.WebApi.Features.Products.Entities;

namespace Postech.Fiap.CartsPayments.WebApi.UnitTests.Mocks;

public static class OrderItemMocks
{
    public static OrderItem GenerateValidOrderItem()
    {
        var faker = new Faker();
        var orderItemId = new OrderItemId(faker.Random.Guid());
        var orderId = new OrderId(faker.Random.Guid());
        var productId = new ProductId(faker.Random.Guid());
        var productName = faker.Commerce.ProductName();
        var unitPrice = faker.Random.Decimal(1, 1000);
        var quantity = faker.Random.Int(1, 100);
        var category = faker.PickRandom<ProductCategory>();

        return OrderItem.Create(orderItemId, orderId, productId, productName, unitPrice, quantity,
            category);
    }

    public static OrderItem GenerateInvalidOrderItem()
    {
        return OrderItem.Create(new OrderItemId(Guid.Empty), new OrderId(Guid.Empty), new ProductId(Guid.Empty),
            string.Empty, -1, 0, ProductCategory.Lanche);
    }
}