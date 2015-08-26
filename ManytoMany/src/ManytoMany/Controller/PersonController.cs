using System.Linq;
using System.Threading.Tasks;
using ManytoMany.Dal;
using ManytoMany.Models;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using EntityState = Microsoft.Data.Entity.EntityState;

namespace ManytoMany.Controller
{
    public class PersonController : Microsoft.AspNet.Mvc.Controller
    {
        private readonly StoreDbContext dbContext;

        public PersonController(StoreDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var allPersons = this.dbContext.Set<Person>().ToList();
            return View(allPersons);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }

            var pers = await this.dbContext.Set<Person>().Include(f => f.PersonPhotos).FirstAsync(p => p.Id == id);
            if (pers == null)
            {
                return this.HttpNotFound();
            }

            var allFotos = this.dbContext.Set<Photo>().ToList();
            var personFotos = (from r in allFotos
                               where pers.PersonPhotos.Any(ap => ap.PhotoId.Equals(r.Id))
                               select r).ToList();

            var model = new PersonViewModel()
            {
                Id = pers.Id,
                Name = pers.Name
            };

            // load the Photos for selection in the form:
            foreach (var foto in allFotos)
            {
                var listItem = new SelectListItem()
                {
                    Text = foto.Name,
                    Value = foto.Id.ToString(),
                    Selected = personFotos.Any(g => g.Id == foto.Id)
                };

                model.PhotosList.Add(listItem);
            }

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,Name")] PersonViewModel model, params string[] selectedPhotos)
        {
            var pers = await this.dbContext.Persons.Include(f => f.PersonPhotos).AsNoTracking().FirstAsync(p => p.Id == model.Id);
            if (pers == null)
            {
                return this.HttpNotFound();
            }

            if (this.ModelState.IsValid)
            {
                pers.Name = model.Name;
                
                this.dbContext.Entry(pers).State = EntityState.Modified;
                this.dbContext.SaveChanges();

                selectedPhotos = selectedPhotos ?? new string[] { };

                var firstOrDefault = this.dbContext.Persons.Include(f => f.PersonPhotos).AsNoTracking().FirstOrDefault(p => p.Id == model.Id);
                if (firstOrDefault != null)
                {
                    // Remove all Photos from Person
                    var personPhotos = firstOrDefault.PersonPhotos.ToList();
                    foreach (var personPhoto in personPhotos)
                    {
                        this.dbContext.Entry(personPhoto).State = EntityState.Deleted;
                        ////this.dbContext.Remove(personPhoto);
                    }
                }
                this.dbContext.SaveChanges();

                var newFotos = this.dbContext.Set<Photo>().Where(p => selectedPhotos.Any(n => n == p.Name));
                foreach (var foto in newFotos)
                {
                    var personPhoto = new PersonPhoto {PersonId = pers.Id, PhotoId = foto.Id};
                    this.dbContext.Entry(personPhoto).State = EntityState.Added;
                }
                this.dbContext.SaveChanges();

                return this.RedirectToAction("Index");
            }

            return this.View(model);
        }
    }
}
