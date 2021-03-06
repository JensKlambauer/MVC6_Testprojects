﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using Microsoft.AspNet.Mvc.ModelBinding;
using MVC6MusicStore.Core.DAL;

namespace MVC6MusicStore.Core.Models
{
    public class Album
    {
        [ScaffoldColumn(false)]
        public int AlbumId { get; set; }

        public int GenreId { get; set; }

        public int ArtistId { get; set; }

        [Required]
        [StringLength(160, MinimumLength = 2)]
        public string Title { get; set; }

        [Required]
        [Range(0.01, 100.00)]

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Display(Name = "Album Art URL")]
        [StringLength(1024)]
        public string AlbumArtUrl { get; set; }

        public virtual Genre Genre { get; set; }

        public virtual Artist Artist { get; set; }

        ////public virtual List<OrderDetail> OrderDetails { get; set; }

        [ScaffoldColumn(false)]
        [BindNever]
        [Required]
        public DateTime Created { get; set; }

        /// <summary>
        /// TODO: Temporary hack to populate the orderdetails until EF does this automatically. 
        /// </summary>
        public Album()
        {
            ////OrderDetails = new List<OrderDetail>();
            Created = DateTime.UtcNow;
        }

        public Album(IDataReader dataReader)
        {
            this.AlbumId = dataReader.GetValue<int>("AlbumId");
            this.GenreId = dataReader.GetValue<int>("GenreId");
            this.ArtistId = dataReader.GetValue<int>("ArtistId");
            this.Title = dataReader.GetValue<string>("Title");
            this.Price = dataReader.GetValue<decimal>("Price");
            this.AlbumArtUrl = dataReader.GetValue<string>("AlbumArtUrl");

            this.Genre = new Genre(dataReader);
            this.Artist = new Artist(dataReader);

            Created = DateTime.UtcNow;
        }
    }
}