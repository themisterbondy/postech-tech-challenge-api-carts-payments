using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Postech.Fiap.CartsPayments.WebApi.Features.Orders.Entities;
using Postech.Fiap.CartsPayments.WebApi.Features.Products.Entities;

namespace Postech.Fiap.CartsPayments.WebApi.Persistence.Configurations;

[ExcludeFromCodeCoverage]
public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.Id)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("uuid_generate_v4()")
            .HasConversion(orderItemId => orderItemId.Value,
                value => new OrderItemId(value));

        builder.Property(oi => oi.ProductId)
            .HasConversion(productId => productId.Value,
                value => new ProductId(value))
            .IsRequired();

        builder.Property(oi => oi.ProductName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(oi => oi.UnitPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(oi => oi.Quantity)
            .IsRequired();

        builder.Property(oi => oi.Category)
            .IsRequired()
            .HasConversion<string>(
                category => category.ToString(),
                value => (ProductCategory)Enum.Parse(typeof(ProductCategory), value));
    }
}