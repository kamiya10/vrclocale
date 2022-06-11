using System.Globalization;
using System.Windows.Controls;

namespace VRCTC_Installer.Validation
{
    public class NotEmptyValidationRule : ValidationRule
    {
        public NotEmptyValidationRule() { }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return value == null ? new ValidationResult(false, "必填欄位") : ValidationResult.ValidResult;
        }
    }
}