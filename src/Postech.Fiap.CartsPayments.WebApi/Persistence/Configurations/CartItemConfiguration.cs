using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Entities;
using Postech.Fiap.CartsPayments.WebApi.Features.Products.Entities;

namespace Postech.Fiap.CartsPayments.WebApi.Persistence.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItems");

        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.Id)
            .HasConversion(id => id.Value, value => new CartItemId(value))
            .IsRequired();

        builder.Property(ci => ci.ProductId)
            .HasConversion(id => id.Value, value => new ProductId(value))
            .IsRequired();

        builder.Property(ci => ci.ProductName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(ci => ci.UnitPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(ci => ci.Quantity)
            .IsRequired();

        builder.Property(ci => ci.CartId)
            .HasConversion(id => id.Value, value => new CartId(value))
            .IsRequired();

        builder.Property(oi => oi.Category)
            .IsRequired()
            .HasConversion<string>(
                category => category.ToString(),
                value => (ProductCategory)Enum.Parse(typeof(ProductCategory), value));

        builder.HasOne<Cart>()
            .WithMany(c => c.Items)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}