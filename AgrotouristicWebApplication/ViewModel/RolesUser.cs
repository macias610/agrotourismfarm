using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ViewModel
{
    public class RolesUser
    {
        public string Id { get; set; }
        [Display(Name ="Email:")]
        public string Email { get; set; }
        [Display(Name ="Imię:")]
        public string Name { get; set; }
        [Display(Name ="Nazwisko:")]
        public string Surname { get; set; }
        [Display(Name ="Data urodzenia:")]
        public DateTime BirthDate { get; set; }
        [Display(Name ="Poziomy dostępu:")]
        public List<string> Roles { get; set; }
    }
}