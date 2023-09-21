using Reference_Books.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reference_Books.Models.Domein
{
    public class Relation
    {
        public int Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PersonId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Target person ID must be a positive integer.")]
        public int TargetPerson { get; set; }

        [EnumDataType(typeof(RelationTypeEnum), ErrorMessage = "Relation type must be 'Friend', 'Colleague' or 'Relative'.")]
        public RelationTypeEnum RelationType { get; set; }
        public Person Person { get; set; } = null!;
    }
}
