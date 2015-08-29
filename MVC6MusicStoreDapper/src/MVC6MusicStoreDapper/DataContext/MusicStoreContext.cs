using Microsoft.Data.Entity;
using MVC6MusicStore.Core.Models;

namespace MVC6MusicStoreDapper.DataContext
{
    public class MusicStoreContext : DbContext
    {
        public DbSet<Album> Albums { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Genre> Genres { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // TODO: Remove when explicit values insertion removed.
            builder.Entity<Artist>().Property(a => a.ArtistId).ValueGeneratedNever();
            builder.Entity<Genre>().Property(g => g.GenreId).ValueGeneratedNever();

            // Deleting an album fails with this relation
            ////builder.Entity<Album>().Ignore(a => a.OrderDetails);
            ////builder.Entity<OrderDetail>().Ignore(od => od.Album);

            base.OnModelCreating(builder);
        }
    }
}