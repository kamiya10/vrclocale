using System;
using System.Globalization;
using System.IO;
using System.Windows.Controls;

namespace VRCTC_Installer.Validation
{
    public class FilePathValidationRule : ValidationRule
    {
        public FilePathValidationRule() { }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                FileInfo file = new FileInfo((string)value);
                if (file != null)
                {
                    if (!((string)value).EndsWith("VRChat.exe"))
                    {
                        return new ValidationResult(false, "檔案不是 VRChat.exe");
                    }
                }
            }
            catch (Exception)
            {
                return new ValidationResult(false, "請輸入正確的路徑");
            }
            return ValidationResult.ValidResult;
        }
    }
}
