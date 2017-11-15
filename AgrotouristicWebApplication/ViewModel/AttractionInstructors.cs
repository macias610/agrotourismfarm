using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ViewModel
{
    public class AttractionInstructors
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

        public IList<SelectListItem> DaysOfWeek { get; set; }

        public Dictionary<DateTime, List<string>> AssignedAttractions { get; set; }

        public int MaxRows { get; set; }
    }
}