using System.ComponentModel.DataAnnotations;
using System;

namespace WebApp.Models
{
    public class PersonDetail
    {
        public int Id { get; set; }

        [Required]
        public int PersonId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime BirthDay { get; set; }

        [Required]
        public string PersonCity { get; set; }

    }
}
