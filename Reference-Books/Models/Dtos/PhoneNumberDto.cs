using System.ComponentModel.DataAnnotations;

namespace Reference_Books.Models.Dtos
{
    public class PhoneNumberDto
    {
        [StringLength(50, MinimumLength = 4, ErrorMessage = "First name must be between 2 and 50 characters.")]
        [RegularExpression(@"^(?:\+\d+|\d+)$", ErrorMessage = "Phone number must contain only '+' and numeric digits.")]
        public string? MobileNumber { get; set; }


        [StringLength(50, MinimumLength = 4, ErrorMessage = "First name must be between 2 and 50 characters.")]
        [RegularExpression(@"^(?:\+\d+|\d+)$", ErrorMessage = "Phone number must contain only '+' and numeric digits.")]
        public string? HomeNumber { get; set; }

        [Phone]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "First name must be between 2 and 50 characters.")]
        [RegularExpression(@"^(?:\+\d+|\d+)$", ErrorMessage = "Phone number must contain only '+' and numeric digits.")]
        public string? OfficeNumber { get; set; }
    }
}
