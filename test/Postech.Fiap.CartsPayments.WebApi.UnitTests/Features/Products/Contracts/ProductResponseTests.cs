using Postech.Fiap.CartsPayments.WebApi.Features.Products.Contracts;
using Postech.Fiap.CartsPayments.WebApi.Features.Products.Entities;

namespace Postech.Fiap.CartsPayments.WebApi.UnitTests.Features.Products.Contracts;

public class ProductResponseTests
{
    [Fact]
    public void ProductResponse_Should_Set_And_Get_Properties_Correctly()
    {
        // Arrange
        var id = Guid.NewGuid();
        const string name = "Cheeseburger";
        const string description = "Delicious cheeseburger with cheddar and bacon";
        const decimal price = 15.99m;
        const ProductCategory category = ProductCategory.Lanche;
        const string imageUrl = "https://example.com/image.png";

        // Act
        var product = new ProductResponse
        {
            Id = id,
            Name = name,
            Description = description,
            Price = price,
            Category = category,
            ImageUrl = imageUrl
        };

        // Assert
        product.Id.Should().Be(id);
        product.Name.Should().Be(name);
        product.Description.Should().Be(description);
        product.Price.Should().Be(price);
        product.Category.Should().Be(category);
        product.ImageUrl.Should().Be(imageUrl);
    }

    [Fact]
    public void ProductResponse_Should_Allow_Null_Description_And_ImageUrl()
    {
        // Act
        var product = new ProductResponse
        {
            Id = Guid.NewGuid(),
            Name = "Soda",
            Description = null,
            Price = 5.00m,
            Category = ProductCategory.Bebida,
            ImageUrl = null
        };

        // Assert
        product.Description.Should().BeNull();
        product.ImageUrl.Should().BeNull();
    }
}