using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using MVC6MusicStore.Core.DAL;

namespace MVC6MusicStore.Core.Models
{
    public class Genre
    {
        public Genre()
        {
        }

        public Genre(IDataReader dataReader)
        {
            this.GenreId = dataReader.GetValue<int>("GenreId");
            this.Name = dataReader.GetValue<string>("Name");
            this.Description = dataReader.GetValue<string>("Description");
        }

        public int GenreId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public List<Album> Albums { get; set; }
    }
}