using Microsoft.AspNet.Identity.EntityFramework;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repository.ViewModels
{
    public class RolesUsers
    {
        public RolesUsers()
        {

        }
        public List<User> Users { get; set; }
        public Dictionary<string,string> Roles { get; set; }
    }
}