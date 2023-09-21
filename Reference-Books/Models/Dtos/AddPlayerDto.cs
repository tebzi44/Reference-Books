using Reference_Books.Models.CustomValidation;
using Reference_Books.Models.Domein;
using System.ComponentModel.DataAnnotations;

namespace Reference_Books.Models.Dtos
{
    public class AddPlayerDto
    {
        [Required(ErrorMessage = "First name is required.")]
        [NoMixedAlphabet(ErrorMessage = "First name cannot contain both Latin and Georgian letters.")]
        [RegularExpression(@"^[a-zA-Zა-ჰ\s]+$", ErrorMessage = "Only English and Georgian characters are allowed.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required.")]
        [NoMixedAlphabet(ErrorMessage = "First name cannot contain both Latin and Georgian letters.")]
        [RegularExpression(@"^[a-zA-Zა-ჰ\s]+$", ErrorMessage = "Only English and Georgian characters are allowed.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender is required.")]
        [EnumDataType(typeof(GenderEnum), ErrorMessage = "Gender must be 'Male' or 'Female'.")]
        public GenderEnum Gender { get; set; }

        [Required(ErrorMessage = "Personal number is required.")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Personal number must contain only numeric characters.")]
        public string PersonalNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Birth day is required.")]
        public DateTime BirthDay { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public int City { get; set; }

        public PhoneNumberDto? PhoneNumbers { get; set; }

        public List<RelationsListDto>? RelatedPeoplesList { get; set; }
    }

}
