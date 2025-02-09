using Postech.Fiap.CartsPayments.WebApi.Common.ResultPattern;

namespace Postech.Fiap.CartsPayments.WebApi.Common.Extensions;

[ExcludeFromCodeCoverage]
public static class ResultExtensions
{
    public static IResult ToProblemDetails(this Result result)
    {
        if (result.IsSuccess) throw new InvalidOperationException();

        var extensions = new Dictionary<string, object?>
        {
            {
                "errors", result.Errors is null || result.Errors.Length == 0
                    ? [result.Error]
                    : result.Errors
            }
        };

        return Results.Problem(
            statusCode: GetStatusCode(result.Error.ErrorType),
            title: GetTitle(result.Error.ErrorType),
            type: GetType(result.Error.ErrorType),
            detail: result.Error.Message,
            extensions: extensions);

        static int GetStatusCode(ErrorType errorType)
        {
            return errorType switch
            {
                ErrorType.Failure => StatusCodes.Status400BadRequest,
                ErrorType.Validation => StatusCodes.Status422UnprocessableEntity,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };
        }

        static string GetTitle(ErrorType errorType)
        {
            return errorType switch
            {
                ErrorType.Failure => "Bad Request",
                ErrorType.Validation => "Unprocessable Content",
                ErrorType.NotFound => "Not Found",
                ErrorType.Conflict => "Conflict",
                _ => "Internal Server Error"
            };
        }

        static string GetType(ErrorType errorType)
        {
            return errorType switch
            {
                ErrorType.Failure => "https://datatracker.ietf.org/doc/html/rfc9110#status.400",
                ErrorType.Validation => "https://datatracker.ietf.org/doc/html/rfc9110#status.422",
                ErrorType.NotFound => "https://datatracker.ietf.org/doc/html/rfc9110#status.404",
                ErrorType.Conflict => "https://datatracker.ietf.org/doc/html/rfc9110#status.409",
                _ => "https://datatracker.ietf.org/doc/html/rfc9110#status.500"
            };
        }
    }
}