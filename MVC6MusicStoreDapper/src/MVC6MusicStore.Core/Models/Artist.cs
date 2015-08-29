using System.ComponentModel.DataAnnotations;

namespace MVC6MusicStore.Core.Models
{
    public class Artist
    {
        public int ArtistId { get; set; }

        [Required]
        public string Name { get; set; }
    }
}