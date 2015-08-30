using System.ComponentModel.DataAnnotations;
using System.Data;
using MVC6MusicStore.Core.DAL;

namespace MVC6MusicStore.Core.Models
{
    public class Artist
    {
        public Artist()
        {
        }

        public Artist(IDataReader dataReader)
        {
            this.ArtistId = dataReader.GetValue<int>("ArtistId");
            this.ArtistName = dataReader.GetValue<string>("ArtistName");
        }

        public int ArtistId { get; set; }

        [Required]
        public string ArtistName { get; set; }
    }
}