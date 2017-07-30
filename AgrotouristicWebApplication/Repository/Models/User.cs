using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Repository.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class User : IdentityUser
    {

        public User()
        {
            this.Reservations = new HashSet<Reservation>();
            this.Attraction_Reservation_Worker = new HashSet<Attraction_Reservation_Worker>();
        }

        [Required]
        [Display(Name = "Imię")]

        public string Name { get; set; }
        [Display(Name = "Nazwisko")]
        [Required]
        public string Surname { get; set; }

        [Display(Name = "Data urodzenia")]
        [DataType(DataType.Date)]
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime BirthDate { get; set; }

        [Display(Name = "Data zatrudnienia")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? HireDate { get; set; }

        [Display(Name = "Profesja")]
        [MinLength(3), MaxLength(15)]
        public string Profession { get; set; }

        [Display(Name = "Pensja")]
        public decimal? Salary { get; set; }

        public virtual ICollection<Attraction_Reservation_Worker> Attraction_Reservation_Worker { get; set; }

        public virtual ICollection<Reservation> Reservations { get; private set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}