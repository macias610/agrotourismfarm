using Microsoft.AspNet.Identity.EntityFramework;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repository.ViewModels
{
    public class RoleUser
    {
        public RoleUser()
        {

        }
        public IEnumerable<SelectListItem> Roles { get; set; }

        public string userId { get; set; }

        public int SelectedRoleId { get; set; }

        public string SelectedRoleText { get; set; }

    }
}