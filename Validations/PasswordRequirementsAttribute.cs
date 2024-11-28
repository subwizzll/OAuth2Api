using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace OAuth2Api.Validations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class PasswordRequirementsAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string password)
            return new ValidationResult("Password is required.");

        if (Regex.IsMatch(password, @"\s"))
            return new ValidationResult("Password must not contain any whitespace characters.");

        if (!Regex.IsMatch(password, @"[A-Z]"))
            return new ValidationResult("Password must contain at least one uppercase letter.");

        if (!Regex.IsMatch(password, @"[0-9]"))
            return new ValidationResult("Password must contain at least one number.");

        if (!Regex.IsMatch(password, @"[\W_]"))
            return new ValidationResult("Password must contain at least one special character.");

        return ValidationResult.Success;
    }
}