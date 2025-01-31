using FluentValidation.TestHelper;
using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Commands;
using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Contracts;

namespace Postech.Fiap.CartsPayments.WebApi.UnitTests.Features.Carts.Commands;

public class AddToCartValidatorTests
{
    private readonly AddItensToCart.AddToCartValidator _validator = new();

    [Fact]
    public void ShouldHaveErrorWhenCustomerIdIsInvalid()
    {
        var command = new AddItensToCart.Command { CustomerId = Guid.Empty };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.CustomerId);
    }

    [Fact]
    public void ShouldNotHaveErrorWhenCustomerIdIsValid()
    {
        var command = new AddItensToCart.Command { CustomerId = Guid.NewGuid() };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.CustomerId);
    }

    [Fact]
    public void ShouldHaveErrorWhenProductIdIsEmpty()
    {
        // Arrange
        var command = new AddItensToCart.Command
        {
            Items = [new CartItemRequest { Quantity = 1 }]
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Items[0].ProductId");
    }

    [Fact]
    public void ShouldNotHaveErrorForValidItems()
    {
        // Arrange
        var command = new AddItensToCart.Command
        {
            CustomerId = Guid.NewGuid(),
            Items = [new CartItemRequest { ProductId = Guid.NewGuid(), Quantity = 1 }]
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor("Items[0].ProductId");
    }

    [Fact]
    public void ShouldHaveErrorWhenQuantityIsZero()
    {
        var command = new AddItensToCart.Command
        {
            CustomerId = Guid.NewGuid(),
            Items = [new CartItemRequest { ProductId = Guid.Empty, Quantity = 0 }]
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor("Items[0].Quantity");
    }

    [Fact]
    public void ShouldNotHaveErrorWhenQuantityIsGreaterThanZero()
    {
        var command = new AddItensToCart.Command
        {
            Items =
                [new CartItemRequest { ProductId = Guid.Empty, Quantity = 0 }]
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Items);
    }
}