using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reference_Books.Models.Domein
{
    public class PhoneNumber
    {
        public int Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PersonId { get; set; }

        [StringLength(50, MinimumLength = 4, ErrorMessage = "First name must be between 2 and 50 characters.")]
        [RegularExpression(@"^(?:\+\d+|\d+)$", ErrorMessage = "Phone number must contain only '+' and numeric digits.")]
        public string? MobileNumber { get; set; } = string.Empty;


        [StringLength(50, MinimumLength = 4, ErrorMessage = "First name must be between 2 and 50 characters.")]
        [RegularExpression(@"^(?:\+\d+|\d+)$", ErrorMessage = "Phone number must contain only '+' and numeric digits.")]
        public string? HomeNumber { get; set; } = string.Empty;

        [StringLength(50, MinimumLength = 4, ErrorMessage = "First name must be between 2 and 50 characters.")]
        [RegularExpression(@"^(?:\+\d+|\d+)$", ErrorMessage = "Phone number must contain only '+' and numeric digits.")]
        public string? OfficeNumber { get; set; } = string.Empty;
        public Person Person { get; set; } = null!;
    }
}
