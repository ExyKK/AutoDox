using System.Globalization;
using System.IO;
using System.Windows.Controls;

namespace AutoDox.UI.Helpers
{
    internal class DestinationDirValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (!Directory.Exists(value.ToString()))
            {
                return new ValidationResult(false, Properties.Resources.DestinationDirectoryInvalid);
            }
            return ValidationResult.ValidResult;
        }
    }
}
