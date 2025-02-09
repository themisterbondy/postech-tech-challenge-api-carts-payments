namespace Postech.Fiap.CartsPayments.WebApi.Common.Validation;

[ExcludeFromCodeCoverage]
public static class GlobalValidations
{
    public static bool BeAValidCpf(string cpf)
    {
        if (cpf.Length != 11)
            return false;

        string[] invalidCpfs =
        [
            "00000000000",
            "11111111111",
            "22222222222",
            "33333333333",
            "44444444444",
            "55555555555",
            "66666666666",
            "77777777777",
            "88888888888",
            "99999999999"
        ];

        if (invalidCpfs.Contains(cpf))
            return false;

        var number = cpf.Select(digit => int.Parse(digit.ToString())).ToArray();
        var sum = 0;
        for (var i = 0; i < 9; i++)
            sum += (10 - i) * number[i];

        var result = sum % 11;
        if (result < 2)
        {
            if (number[9] != 0)
                return false;
        }
        else if (number[9] != 11 - result)
        {
            return false;
        }

        sum = 0;
        for (var i = 0; i < 10; i++)
            sum += (11 - i) * number[i];

        result = sum % 11;
        if (result < 2)
        {
            if (number[10] != 0)
                return false;
        }
        else if (number[10] != 11 - result)
        {
            return false;
        }

        return true;
    }
}