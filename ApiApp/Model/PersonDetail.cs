using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Internship.Model
{
    public class PersonDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Person")]
        public int PersonId { get; set; }

        public virtual Person Person { get; set; }

        [Required]
        public DateTime BirthDay { get; set; }

        [Required]
        public string PersonCity { get; set; }
    }
}
