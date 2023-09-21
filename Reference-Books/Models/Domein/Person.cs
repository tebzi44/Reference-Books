using Reference_Books.Models.CustomValidation;
using System.ComponentModel.DataAnnotations;

namespace Reference_Books.Models.Domein

{
    public enum GenderEnum
    {
        [Display(Name = "Male")]
        Male,

        [Display(Name = "Female")]
        Female
    }

    public class Person
    {
        public int Id { get; set; }
        [NoMixedAlphabet(ErrorMessage = "First name cannot contain both Latin and Georgian letters.")]
        [RegularExpression(@"^[a-zA-Zა-ჰ\s]+$", ErrorMessage = "Only English and Georgian characters are allowed.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [NoMixedAlphabet(ErrorMessage = "First name cannot contain both Latin and Georgian letters.")]
        [RegularExpression(@"^[a-zA-Zა-ჰ\s]+$", ErrorMessage = "Only English and Georgian characters are allowed.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
        public string LastName { get; set; } = string.Empty;

        [EnumDataType(typeof(GenderEnum), ErrorMessage = "Gender must be 'Male' or 'Female'.")]
        public GenderEnum Gender { get; set; }

        public string PersonalNumber { get; set; }

        public DateTime BirthDay { get; set; }

        public int City { get; set; }

        public PhoneNumber? PhoneNumbers { get; set; }

        public string? PhotoUrl { get; set; }

        public ICollection<Relation>? Relations { get; set; } = new List<Relation>();
    }
}
