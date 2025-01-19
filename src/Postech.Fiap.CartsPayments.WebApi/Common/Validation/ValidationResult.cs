using Postech.Fiap.CartsPayments.WebApi.Common.ResultPattern;

namespace Postech.Fiap.CartsPayments.WebApi.Common.Validation;

/// <summary>
///     Represents the result of a validation.
/// </summary>
[ExcludeFromCodeCoverage]
public class ValidationResult : Result, IValidationResult
{
    /// <summary>
    ///     Represents the result of a validation operation.
    /// </summary>
    private ValidationResult(Error[]? errors) : base(false, IValidationResult.ValidationError, errors!)
    {
    }

    /// <summary>
    ///     Creates a new instance of ValidationResult with the specified array of errors.
    /// </summary>
    /// <param name="errors">An array of errors to be associated with the validation result.</param>
    /// <returns>A ValidationResult object with the specified array of errors.</returns>
    public static ValidationResult WithErrors(Error[] errors)
    {
        return new ValidationResult(errors);
    }
}