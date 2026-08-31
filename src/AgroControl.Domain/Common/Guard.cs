namespace AgroControl.Domain.Common;

public static class Guard
{
    public static string AgainstNullOrWhiteSpace(
        string? value,
        string parameterName,
        int? maximumLength = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("The value cannot be null, empty or whitespace.", parameterName);
        }

        var normalizedValue = value.Trim();

        if (maximumLength.HasValue && normalizedValue.Length > maximumLength.Value)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                normalizedValue.Length,
                $"The value cannot exceed {maximumLength.Value} characters.");
        }

        return normalizedValue;
    }

    public static decimal AgainstNegative(decimal value, string parameterName)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, value, "The value cannot be negative.");
        }

        return value;
    }

    public static Guid AgainstEmpty(Guid value, string parameterName)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("The identifier cannot be empty.", parameterName);
        }

        return value;
    }
}
