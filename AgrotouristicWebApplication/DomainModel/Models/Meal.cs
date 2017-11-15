using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DomainModel.Models
{
    public class Meal
    {

        public Meal()
        {
            this.Reservation_House = new HashSet<Reservation_House>();
        }

        [Display(Name = "Id:")]
        [Required]
        public int Id { get; set; }

        [Display(Name = "Typ:")]
        [Index(IsUnique = true)]
        [MinLength(10),MaxLength(100)]
        [RegularExpression(@"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ]{5,15}(?:\+[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ]{5,15}){0,2}$")]
        [Required]
        public string Type { get; set; }

        [Display(Name = "Cena:")]
        [Required]
        public decimal Price { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual ICollection<Reservation_House> Reservation_House { get; set; }
    }
}