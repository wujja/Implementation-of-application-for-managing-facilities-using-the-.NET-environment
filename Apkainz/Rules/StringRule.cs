using System.Globalization;
using System.Windows.Controls;

namespace Apkainz
{
    public class StringRule : ValidationRule
    {
        public int MinValue { get; set; }

        public StringRule() { }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (((string)value).Length < MinValue)
            {
                return new ValidationResult(false,
               "Please enter text, at least: " + MinValue + " characters.");
            }
            else
            {
                return ValidationResult.ValidResult;
            }
        }
    }
}
