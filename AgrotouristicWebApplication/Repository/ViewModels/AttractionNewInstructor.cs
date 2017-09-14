using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repository.ViewModels
{
    public class AttractionNewInstructor
    {
        [Display(Name ="Termin warsztatu")]
        public DateTime TermAffair { get; set; }

        [Display(Name ="Nazwa atrakcji")]
        public string AttractionName { get; set; }

        [Display(Name ="Liczba uczestników")]
        public int QuantityParticipant { get; set; }

        [Display(Name ="Wybrany instruktor")]
        public List<SelectListItem> AvaiableInstructors { get; set; }

        public int AttractionReservationID { get; set; }
    }
}