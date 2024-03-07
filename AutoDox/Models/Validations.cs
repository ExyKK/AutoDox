using System.Globalization;
using System.IO;
using System.Windows.Controls;

namespace AutoDox.Models
{
    internal class DestinationDirValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (!Directory.Exists(value.ToString()))
            {
                return new ValidationResult(false, "Please enter a path to existing directory");
            }
            return ValidationResult.ValidResult;          
        }
    }
}
