using System.Globalization;
using System.IO;
using System.Windows.Controls;

namespace MossWPF.Core.Validation
{
    public class FolderPathValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null) { return ValidationResult.ValidResult; }
            return Directory.Exists(value.ToString()) 
                ? ValidationResult.ValidResult 
                : new ValidationResult(false, "Directory does not exist.");
        }
    }
}
