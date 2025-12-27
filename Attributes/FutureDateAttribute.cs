using System.ComponentModel.DataAnnotations;

namespace TournamentApi.Attributes;

public class FutureDateAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null)
        {
            return false;
        }

        if (value is DateTime dateTime)
        {
            return dateTime > DateTime.UtcNow;
        }

        if (value is DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset > DateTimeOffset.UtcNow;
        }

        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} musi być datą w przyszłości.";
    }
}

