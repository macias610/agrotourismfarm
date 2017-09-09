using Repository.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repository.ViewModels
{
    public class ReservationAttractions
    {
        public static Dictionary<string, string> Days = new Dictionary<string, string>()
        {
            {"Monday","Poniedziałek"},
            {"Tuesday","Wtorek" },
            {"Wednesday","Środa" },
            {"Thursday","Czwartek" },
            {"Friday","Piątek" },
            {"Saturday","Sobota" },
            {"Sunday","Niedziela" }
        };

        public decimal OverallCost { get; set; }

        public List<SelectListItem> DaysOfWeek { get; set; }

        public List<SelectListItem> AvaiableAttractions { get; set; }

        public List<SelectListItem> ParticipantsQuantity { get; set; }

        public Dictionary<DateTime,List<string>> AssignedAttractions { get; set; }

        public int MaxRows { get; set; }
    }
}