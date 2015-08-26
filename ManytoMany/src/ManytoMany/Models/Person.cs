using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ManytoMany.Models
{
    public class Person
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<PersonPhoto> PersonPhotos { get; set; } = new List<PersonPhoto>();
    }
}