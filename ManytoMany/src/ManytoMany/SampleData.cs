using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Framework.DependencyInjection;
using ManytoMany.Dal;
using ManytoMany.Models;

namespace ManytoMany
{
    public static class SampleData
    {
        public static async Task InitializeIdentityDatabaseAsync(IServiceProvider serviceProvider)
        {
            using (var db = serviceProvider.GetRequiredService<StoreDbContext>())
            {
                ////await db.Database.EnsureDeletedAsync();
                if (await db.Database.EnsureCreatedAsync())
                {
                    await InsertTestData(serviceProvider);
                }
            }
        }

        private static async Task InsertTestData(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<StoreDbContext>();

            var photos = new List<Photo> { new Photo { Id = 1, Name = "Photo 1"}, new Photo { Id = 2, Name = "Photo 2" } };
            ////context.Photos.AddRange(photos);
            foreach (var photo in photos)
            {
                context.Entry(photo).State = EntityState.Added;
            }

            await context.SaveChangesAsync();

            var person = new Person { Id = 1, Name = "Je_Kl" };
            ////var persons = new List<Person> { person };
            ////context.Persons.AddRange(persons);
            context.Entry(person).State = EntityState.Added;
            await context.SaveChangesAsync();

            foreach (var photo in photos)
            {
                var personPhoto = new PersonPhoto { PersonId = person.Id, PhotoId = photo.Id };
                context.Entry(personPhoto).State = EntityState.Added;
            }
            await context.SaveChangesAsync();

            var photos2 = new List<Photo> { new Photo { Id = 3, Name = "Photo 3" }, new Photo { Id = 4, Name = "Photo 4" } };
            foreach (var photo in photos2)
            {
                context.Entry(photo).State = EntityState.Added;
            }
            await context.SaveChangesAsync();
        }
    }
}