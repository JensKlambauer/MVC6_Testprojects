using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Microsoft.Framework.Caching.Memory;
using Microsoft.Framework.DependencyInjection;
using Xunit;
using ManytoMany.Dal;
using ManytoMany.Models;
using ManytoMany.Controller;

namespace ManyToMany.Test.Controller
{
    public class PersonControllerTest
    {
        private readonly IServiceProvider serviceProvider;

        public PersonControllerTest()
        {
            var services = new ServiceCollection();

            services.AddEntityFramework()
                      .AddInMemoryDatabase()
                      .AddDbContext<StoreDbContext>(options => options.UseInMemoryDatabase());

            serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public void Index_IndexViewPersons()
        {
            // Arrange
            var dbContext = serviceProvider.GetRequiredService<StoreDbContext>();
            CreateTestPersonWithPhotos(dbContext);

            var controller = new PersonController(dbContext);

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);

            Assert.NotNull(viewResult.ViewData);
            var viewModel = Assert.IsType<List<Person>>(viewResult.ViewData.Model);
            Assert.Equal(1, viewModel.Count);
        }

        [Fact]
        public async Task Edit_EditViewPersonWithPhotos()
        {
            // Arrange
            var dbContext = serviceProvider.GetRequiredService<StoreDbContext>();
            CreateTestPersonWithPhotos(dbContext);

            var controller = new PersonController(dbContext);

            // Act
            var result = await controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);

            Assert.NotNull(viewResult.ViewData);
            var viewModel = Assert.IsType<PersonViewModel>(viewResult.ViewData.Model);
            Assert.Equal(1, viewModel.Id);
            Assert.Equal(4, viewModel.PhotosList.Count);
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