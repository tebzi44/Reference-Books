using Reference_Books.Models.CustomValidation;
using Reference_Books.Models.Domein;
using System.ComponentModel.DataAnnotations;

namespace Reference_Books.Models.Dtos
{
    public class UpdatePersonDto
    {

        [NoMixedAlphabet(ErrorMessage = "First name cannot contain both Latin and Georgian letters.")]
        [RegularExpression(@"^[a-zA-Zა-ჰ\s]+$", ErrorMessage = "Only English and Georgian characters are allowed.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
        public string? FirstName { get; set; }

        [NoMixedAlphabet(ErrorMessage = "First name cannot contain both Latin and Georgian letters.")]
        [RegularExpression(@"^[a-zA-Zა-ჰ\s]+$", ErrorMessage = "Only English and Georgian characters are allowed.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
        public string? LastName { get; set; }

        [EnumDataType(typeof(GenderEnum), ErrorMessage = "Gender must be 'Male' or 'Female'.")]
        public GenderEnum? Gender { get; set; }

        public string? PersonalNumber { get; set; }

        public DateTime? BirthDay { get; set; }

        public int? City { get; set; }

        public PhoneNumberDto? PhoneNumbers { get; set; }

    }
}
