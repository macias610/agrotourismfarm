using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AgrotouristicWebApplication.Models
{
    public class Participant
    {

        public Participant()
        {
            this.Reservation_House_Participant = new HashSet<Reservation_House_Participant>();
        }

        [Display(Name ="Id:")]
        [Required]
        public int Id { get; set; }

        [Display(Name ="Imię:")]
        [Required]
        [MinLength(3),MaxLength(15)]
        public string Name { get; set; }

        [Display(Name ="Nazwisko:")]
        [Required]
        [MinLength(3),MaxLength(15)]
        public string Surname  { get; set; }

        [Display(Name ="Data urodzenia:")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0:dd-MM-yyyy}",ApplyFormatInEditMode =true)]
        public DateTime BirthDate { get; set; }

        public virtual ICollection<Reservation_House_Participant> Reservation_House_Participant { get; set; }

    }
}