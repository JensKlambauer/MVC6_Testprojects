using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Mvc.Rendering;

namespace ManytoMany.Models
{
    public class PersonViewModel
    {
        public PersonViewModel()
        {
            this.PhotosList = new List<SelectListItem>();
        }

        [Required]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        public ICollection<SelectListItem> PhotosList { get; set; }
    }
}