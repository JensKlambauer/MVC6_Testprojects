using System;
using ManytoMany.Models;
using Microsoft.Data.Entity;
using System.Linq;
using Microsoft.Data.Entity.Infrastructure;

namespace ManytoMany.Dal
{
    public class StoreDbContext : DbContext
    {
        public DbSet<Photo> Photos { get; set; }

        public DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Photo>(e =>
            {
                e.HasKey(k => k.Id);
                e.Property(a => a.Id).ValueGeneratedNever();
            });
            builder.Entity<Person>(e =>
            {
                e.HasKey(k => k.Id);
                e.Property(a => a.Id).ValueGeneratedNever();
            });

            builder.Entity<PersonPhoto>(e =>
            {
                e.HasKey(x => new { x.PhotoId, x.PersonId });
                ////e.Reference(a => a.Person).InverseCollection(p => p.PersonPhotos).ForeignKey(d => d.PersonId);
                ////e.Reference(a => a.Photo).InverseCollection(p => p.PersonPhotos).ForeignKey(d => d.PhotoId);
            });

            base.OnModelCreating(builder);
        }

        ////public override int SaveChanges()
        ////{
        ////    this.ChangeTracker.AutoDetectChangesEnabled = false;
        ////    this.ChangeTracker.DetectChanges();

        ////    var entries = this.ChangeTracker.Entries<PersonPhoto>().Where(e => e.State == EntityState.Deleted);

        ////    foreach (var entrie in entries)
        ////    {
        ////        var pers = this.Persons.FirstOrDefault(p => p.Id == entrie.Entity.PersonId);
        ////        this.ChangeTracker.TrackGraph(pers, e => e.State = EntityState.Deleted);
        ////    }

        ////    return base.SaveChanges();
        ////}
    }
}