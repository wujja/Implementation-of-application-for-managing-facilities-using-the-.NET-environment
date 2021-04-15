using System;
using System.Globalization;
using System.Windows.Controls;

namespace Apkainz
{
    public class DecimalRule : ValidationRule
    {
        public int MaxLength { get; set; }
        public int MinLength { get; set; }

        public DecimalRule() { }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            decimal number = 0;
            try
            {
                if (((string)value).Length > 0)
                    number = Decimal.Parse((String)value);
            }
            catch (Exception e)
            {
                return new ValidationResult(false, "Illegal characters or " + e.Message);
            }
            if (((string)value).Length > MaxLength || ((string)value).Length < MinLength)
            {
                return new ValidationResult(false,
               "Please enter an price in the range: " + MinLength + "-" + MaxLength + ".");
            }
            else if (number <= 0)
            {
                return new ValidationResult(false,
                   "Please enter correct price:");
            }
            else
            {
                return ValidationResult.ValidResult;
            }
        }
    }
}
