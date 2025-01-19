using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Postech.Fiap.CartsPayments.WebApi.Features.Orders.Entities;

namespace Postech.Fiap.CartsPayments.WebApi.Persistence.Configurations;

[ExcludeFromCodeCoverage]
public class OrderConfiguration : IEntityTypeConfiguration<OrderQueue>
{
    public void Configure(EntityTypeBuilder<OrderQueue> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("uuid_generate_v4()")
            .HasConversion(orderId => orderId.Value,
                value => new OrderId(value));

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (OrderQueueStatus)Enum.Parse(typeof(OrderQueueStatus), v));

        builder.Property(o => o.CustomerId)
            .HasMaxLength(11);

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(oi => oi.OrderId);

        builder.Property(c => c.TransactionId)
            .IsRequired(false)
            .HasMaxLength(36);
    }
}