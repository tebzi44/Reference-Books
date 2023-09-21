using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Reference_Books.Models.CustomValidation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NoMixedAlphabetAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var inputValue = value as string;

            if (!string.IsNullOrWhiteSpace(inputValue))
            {
                var containsLatin = Regex.IsMatch(inputValue, @"[a-zA-Z]");
                var containsGeorgian = Regex.IsMatch(inputValue, @"[ა-ჰ]");

                if (containsLatin && containsGeorgian)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
