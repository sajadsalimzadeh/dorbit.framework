using System;
using System.ComponentModel.DataAnnotations;

namespace Dorbit.Framework.Validators;

public class DateMaxAttribute(int maxYear, int maxMonth, int maxDay) : ValidationAttribute
{
    private DateTime Max { get; } = new(maxYear, maxMonth, maxDay);

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is DateTime date)
        {
            if (date < Max)
            {
                return new ValidationResult($"Date must be less than {Max:yyyy-MM-dd}");
            }
        }

        return ValidationResult.Success;
    }
}