using Reference_Books.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Reference_Books.Models.Dtos
{
    public class UpdateRelationDto
    {
        public int TargetPerson { get; set; }

        [Required(ErrorMessage = "Relation type is required.")]
        [EnumDataType(typeof(RelationTypeEnum), ErrorMessage = "Relation type must be 'Friend', 'Colleague' or 'Relative'.")]
        public RelationTypeEnum RelationType { get; set; }

        [Required(ErrorMessage = "Action type is required.")]
        [EnumDataType(typeof(UpdateRelationAction), ErrorMessage = "Relation type must be 'Add' or 'Delete'.")]
        public UpdateRelationAction Action { get; set; }
    }
}
