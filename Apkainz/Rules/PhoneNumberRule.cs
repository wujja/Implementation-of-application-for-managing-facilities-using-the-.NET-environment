using System;
using System.Globalization;
using System.Windows.Controls;

namespace Apkainz
{
    public class PhoneNumberRule : ValidationRule
    {
        public int MaxLength { get; set; }
        public int MinLength { get; set; }

        public PhoneNumberRule() { }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int phoneNumber = 0;
            try
            {
                if (((string)value).Length > 0)
                    phoneNumber = Int32.Parse((String)value);
            }
            catch (Exception e)
            {
                return new ValidationResult(false, "Illegal characters or " + e.Message);
            }
            if (((string)value).Length > MaxLength || ((string)value).Length < MinLength)
            {
                return new ValidationResult(false,
               "Please enter an phone number in the range: " + MinLength + "-" + MaxLength + ".");
            }
            else
            {
                return ValidationResult.ValidResult;
            }
        }
    }
}
