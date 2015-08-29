using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Microsoft.Framework.Caching.Memory;
using Microsoft.Framework.DependencyInjection;
using Xunit;
using ManytoMany.Dal;
using ManytoMany.Models;

namespace ManyToMany.Test
{
    public class PersonDbContextTest
    {
        private readonly IServiceProvider serviceProvider;

        public PersonDbContextTest()
        {
            var services = new ServiceCollection();

            services.AddEntityFramework()
                      .AddInMemoryDatabase()
                      .AddDbContext<StoreDbContext>(options => options.UseInMemoryDatabase());

            serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public void GetPersonPhotos()
        {
            // Arrange
            var dbContext = serviceProvider.GetRequiredService<StoreDbContext>();
            CreateTestPersonWithPhotos(dbContext);

            // Act
            var result = dbContext.Set<Person>().Include(f => f.PersonPhotos).First(p => p.Id == 1);

            // Assert
            var viewResult = Assert.IsType<Person>(result);
            Assert.NotNull(viewResult.Id);

            Assert.NotNull(viewResult.PersonPhotos);
            var viewModel = Assert.IsType<List<PersonPhoto>>(viewResult.PersonPhotos);
            Assert.Equal(2, viewModel.Count);
        }

        [Fact]
        public void EditPersonPhotos()
        {
            // Arrange
            var dbContext = serviceProvider.GetRequiredService<StoreDbContext>();
            CreateTestPersonWithPhotos(dbContext);

            // Act
            var result = dbContext.Persons.Include(f => f.PersonPhotos).AsNoTracking().First(p => p.Id == 1);

            // Assert
            var viewResult = Assert.IsType<Person>(result);
            Assert.NotNull(viewResult.Id);

            Assert.NotNull(viewResult.PersonPhotos);
            var viewModel = Assert.IsType<List<PersonPhoto>>(viewResult.PersonPhotos);
            Assert.Equal(2, viewModel.Count);

            // Edit Person -> Remove all Photos from Person, without AsNoTracking()
            var first = dbContext.Persons.Include(f => f.PersonPhotos).First(p => p.Id == 1);
            if (first != null)
            {
                first.PersonPhotos.ToList().ForEach(pp => dbContext.Entry(pp).State = EntityState.Deleted);
            }
            dbContext.SaveChanges();

            // Read Person, only with AsNoTracking() passed the Test
            var pers = dbContext.Persons.Include(f => f.PersonPhotos).AsNoTracking().First(p => p.Id == 1);
            Assert.Equal(0, pers.PersonPhotos.Count);
        }

        private static void CreateTestPersonWithPhotos(DbContext context)
        {
            var photos = new List<Photo> { new Photo { Id = 1, Name = "Photo 1" }, new Photo { Id = 2, Name = "Photo 2" } };
            foreach (var photo in photos)
            {
                context.Entry(photo).State = EntityState.Added;
            }
            context.SaveChanges();

            var person = new Person { Id = 1, Name = "Je_Kl" };
            context.Entry(person).State = EntityState.Added;
            context.SaveChanges();

            foreach (var photo in photos)
            {
                var personPhoto = new PersonPhoto { PersonId = person.Id, PhotoId = photo.Id };
                context.Entry(personPhoto).State = EntityState.Added;
            }
            context.SaveChanges();

            var photos2 = new List<Photo> { new Photo { Id = 3, Name = "Photo 3" }, new Photo { Id = 4, Name = "Photo 4" } };
            foreach (var photo in photos2)
            {
                context.Entry(photo).State = EntityState.Added;
            }
            context.SaveChanges();
        }
    }
}