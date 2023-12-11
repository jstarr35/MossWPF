using System.Globalization;
using System.Windows.Controls;

namespace MossWPF.Core.Validation
{
    public class PositiveIntegerValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return int.TryParse(value as string, out int result) ? ValidationResult.ValidResult : new ValidationResult(false, "Must be a positive integer.");
        }
    }
}
