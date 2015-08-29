using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MVC6MusicStore.Core.Models
{
    public class Genre
    {
        public int GenreId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public List<Album> Albums { get; set; }
    }
}