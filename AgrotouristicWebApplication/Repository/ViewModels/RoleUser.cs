using Microsoft.AspNet.Identity.EntityFramework;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repository.ViewModels
{
    public class RoleUser
    {
        public IEnumerable<SelectListItem> Roles { get; set; }

        [Display(Name ="Identyfikator")]
        public string userId { get; set; }

        [Display(Name ="Poziom dostępu")]
        public string SelectedRoleText { get; set; }

    }
}