using System.Collections.Generic;
using Microsoft.Data.Entity.Metadata;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManytoMany.Models
{
    public class Photo
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        ////[ForeignKey("PhotoId")]
        public ICollection<PersonPhoto> PersonPhotos { get; set; } = new List<PersonPhoto>();
    }
}