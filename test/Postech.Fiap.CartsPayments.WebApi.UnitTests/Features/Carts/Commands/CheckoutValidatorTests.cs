using FluentValidation.TestHelper;
using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Commands;

namespace Postech.Fiap.CartsPayments.WebApi.UnitTests.Features.Carts.Commands;

public class CheckoutValidatorTests
{
    private readonly Checkout.Validator _validator = new();

    [Fact]
    public void ShouldHaveErrorWhenCustomerIdIsInvalid()
    {
        var command = new Checkout.Command { CartId = Guid.Empty };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.CartId);
    }

    [Fact]
    public void ShouldNotHaveErrorWhenCustomerIdIsValid()
    {
        var command = new Checkout.Command { CartId = Guid.NewGuid() };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.CartId);
    }
}