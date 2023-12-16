using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace MossWPF.Core.Validation
{
    public class UserIdValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            // Define the regex pattern
            string pattern = @"^\d{9}$";
            if (value == null) { return ValidationResult.ValidResult; }
            // Use Regex.IsMatch to check if the input matches the pattern
            return Regex.IsMatch(value.ToString(), pattern) ? ValidationResult.ValidResult : new ValidationResult(false, "Must be a 9-digit numeric ID");
        }
    }
}
