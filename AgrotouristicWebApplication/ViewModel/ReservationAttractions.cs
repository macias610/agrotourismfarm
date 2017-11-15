using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ViewModel
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

        public IList<SelectListItem> DaysOfWeek { get; set; }

        public IList<SelectListItem> AvaiableAttractions { get; set; }

        public IList<SelectListItem> ParticipantsQuantity {get;set;}

        public Dictionary<DateTime,List<string>> AssignedAttractions { get; set; }

        public int MaxRows { get; set; }
    }
}