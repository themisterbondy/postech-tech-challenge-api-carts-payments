using Postech.Fiap.CartsPayments.WebApi.Features.Products.Entities;

namespace Postech.Fiap.CartsPayments.WebApi.UnitTests.Features.Products.Entities;

public class ProductIdTests
{
    [Fact]
    public void ProductId_Should_Store_Guid_Value_Correctly()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var productId = new ProductId(guid);

        // Assert
        productId.Value.Should().Be(guid);
    }

    [Fact]
    public void New_Should_Generate_Unique_Guid()
    {
        // Act
        var productId1 = ProductId.New();
        var productId2 = ProductId.New();

        // Assert
        productId1.Should().NotBe(productId2);
        productId1.Value.Should().NotBe(Guid.Empty);
        productId2.Value.Should().NotBe(Guid.Empty);
    }
}