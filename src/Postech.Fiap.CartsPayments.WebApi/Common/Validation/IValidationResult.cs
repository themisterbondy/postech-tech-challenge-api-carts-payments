namespace Postech.Fiap.CartsPayments.WebApi.Common.Validation;

/// <summary>
///     Represents the result of a data validation.
/// </summary>
public interface IValidationResult
{
    /// <summary>
    ///     Represents a validation error that occurred during data validation.
    /// </summary>
    ///
    public static readonly Error ValidationError = Error.Validation(
        "ValidationError",
        "A validation problem occurred.");

    /// <summary>
    ///     Gets the list of errors.
    /// </summary>
    /// <remarks>
    ///     This property returns an array of Error objects that represent any errors that occurred during the execution of a
    ///     method or operation.
    /// </remarks>
    /// <returns>
    ///     An array of Error objects.
    /// </returns>
    Error[]? Errors { get; }
}