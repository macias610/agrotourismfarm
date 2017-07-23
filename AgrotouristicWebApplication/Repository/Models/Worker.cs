using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Repository.Models
{
    [Table("Worker")]
    public class Worker: ApplicationUser
    {
        public Worker()
        {
            this.Attraction_Reservation_Worker = new HashSet<Attraction_Reservation_Worker>();
        }

        [Display(Name = "Data zatrudnienia:")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime HireDate { get; set; }

        [Display(Name = "Profesja:")]
        [MinLength(3),MaxLength(15)]
        [Required]
        public string  Profession { get; set; }

        public virtual ICollection<Attraction_Reservation_Worker> Attraction_Reservation_Worker { get;  set; }
    }
}