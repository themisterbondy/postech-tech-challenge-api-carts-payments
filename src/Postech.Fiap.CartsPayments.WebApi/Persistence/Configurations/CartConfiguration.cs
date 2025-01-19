using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Entities;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Emun;

namespace Postech.Fiap.CartsPayments.WebApi.Persistence.Configurations;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Carts");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasConversion(id => id.Value, value => new CartId(value))
            .IsRequired();

        builder.Property(c => c.CustomerId)
            .HasMaxLength(36);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(o => o.PaymentStatus)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (PaymentStatus)Enum.Parse(typeof(PaymentStatus), v));

        builder.Property(c => c.TransactionId)
            .IsRequired(false)
            .HasMaxLength(36);

        builder.HasMany(c => c.Items)
            .WithOne()
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(c => c.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Property);
    }
}