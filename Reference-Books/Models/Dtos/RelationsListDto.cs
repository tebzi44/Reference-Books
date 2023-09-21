using Reference_Books.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Reference_Books.Models.Dtos
{
    public class RelationsListDto
    {
        [Required(ErrorMessage = "Relation type is required.")]
        [EnumDataType(typeof(RelationTypeEnum), ErrorMessage = "Relation type must be 'Friend', 'Colleague' or 'Relative'.")]
        public RelationTypeEnum RelationType { get; set; }

        [Required(ErrorMessage = "Target person ID is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Target person ID must be a positive integer.")]

        public int TargetPerson { get; set; }
    }
}
