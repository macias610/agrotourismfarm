using Repository.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repository.ViewModels
{
    public class EditionStatusReservation
    {

        [Display(Name = "Id:")]
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Imię")]
        public string Name { get; set; }

        [Display(Name = "Nazwisko")]
        [Required]
        public string Surname { get; set; }

        [Display(Name = "Data przyjazdu:")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Data wyjazdu:")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [Display(Name ="Status")]
        [Required]
        public string Status { get; set; }

        [Display(Name ="Wybrany status:")]
        public IEnumerable<SelectListItem> States { get; set; }
    }
}