using System;
using System.ComponentModel.DataAnnotations;

namespace Dorbit.Framework.Validators;

public class DateMinFromNowAttribute(int minYear, int minMonth, int minDay) : ValidationAttribute
{
    private DateTime Min { get; } = DateTime.UtcNow.Subtract(new TimeSpan(minYear, minMonth, minDay));

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is DateTime date)
        {
            if (date < Min)
            {
                return new ValidationResult($"Date must be greater than {Min:yyyy-MM-dd}");
            }
        }

        return ValidationResult.Success;
    }
}