using System;
using System.ComponentModel.DataAnnotations;

namespace Dorbit.Framework.Validators;

public class DateRangeAttribute(int minYear, int minMonth, int minDay, int maxYear, int maxMonth, int maxDay)
    : ValidationAttribute
{
    private DateTime Min { get; } = new(minYear, minMonth, minDay);
    private DateTime Max { get; } = new(maxYear, maxMonth, maxDay);

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is DateTime date)
        {
            if (date < Min || date > Max)
            {
                return new ValidationResult($"Date must be between {Min:yyyy-MM-dd} and {Max:yyyy-MM-dd}");
            }
        }

        return ValidationResult.Success;
    }
}