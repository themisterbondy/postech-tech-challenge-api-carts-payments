using Postech.Fiap.CartsPayments.WebApi.Common.ResultPattern;

namespace Postech.Fiap.CartsPayments.WebApi.Common.Validation;

/// <summary>
///     Represents the result of a validation operation.
/// </summary>
/// <typeparam name="TValue">The type of the validated value.</typeparam>
[ExcludeFromCodeCoverage]
public class ValidationResult<TValue> : Result<TValue>, IValidationResult
{
    /// <summary>
    ///     Represents the result of a validation process.
    /// </summary>
    /// <param name="errors">An array of Error objects representing the validation errors.</param>
    private ValidationResult(Error[] errors) : base(default, false, IValidationResult.ValidationError, errors)
    {
    }

    /// <summary>
    ///     Creates a new instance of ValidationResult with the specified errors.
    /// </summary>
    /// <param name="errors">An array of Error objects.</param>
    /// <returns>A ValidationResult object.</returns>
    public static ValidationResult<TValue> WithErrors(Error[] errors)
    {
        return new ValidationResult<TValue>(errors);
    }
}