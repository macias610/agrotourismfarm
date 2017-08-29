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
            AssignedHousesMeals = new Dictionary<string, int>();
            AssignedParticipantsHouses = new Dictionary<string, List<Participant>>();
            stagesConfirmation = new List<bool>(4);
            stagesConfirmation.AddRange(Enumerable.Repeat(false, 4));
            OverallCost = 0;
        }

        public Dictionary<string,int> AssignedHousesMeals { get; set; }

        public Dictionary<string,List<Participant>> AssignedParticipantsHouses { get; set; }

        public List<bool> stagesConfirmation { get; set; }

        public decimal OverallCost { get; set; }

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