using Repository.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repository.ViewModels
{
    public class NewReservation
    {
        public NewReservation()
        {
            SelectedHouses = new List<SelectListItem>();
            SelectedHousesMeals = new List<SelectListItem>();
            AssignedHousesMeals = new Dictionary<int, int>();
            AssignedParticipantsHouses = new Dictionary<string, List<Participant>>();
        }

        public IEnumerable<SelectListItem> AvaiableHouses { get; set; }

        public IEnumerable<SelectListItem> SelectedHouses { get; set; }

        public IEnumerable<SelectListItem> AvaiableMeals { get; set; }

        public IEnumerable<SelectListItem> SelectedHousesMeals { get; set; }

        public Dictionary<int,int> AssignedHousesMeals { get; set; }

        public Dictionary<string,List<Participant>> AssignedParticipantsHouses { get; set; }

        [Display(Name = "Data przyjazdu:")]
        [DataType(DataType.Date)]
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Data odjazdu:")]
        [DataType(DataType.Date)]
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }
    }
}