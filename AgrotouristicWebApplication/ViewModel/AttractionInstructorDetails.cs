using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ViewModel
{
    public class AttractionInstructorDetails
    {
        [Display(Name = "Termin warsztatu")]
        public DateTime TermAffair { get; set; }

        [Display(Name = "Nazwa warsztatu")]
        public string AttractionName { get; set; }

        [Display(Name = "Liczba uczestników")]
        public int QuantityParticipant { get; set; }

        [Display(Name = "Instruktor")]
        public string Instructor { get; set; }

        public int AttractionReservationID { get; set; }

        public int AttractionReservationWorkerID { get; set; }
    }
}