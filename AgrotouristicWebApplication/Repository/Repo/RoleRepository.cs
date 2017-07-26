using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Repository.IRepo;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Repository.Repository
{
    public class RoleRepository: IRoleRepository
    {
        private readonly AgrotourismContext db;

        public RoleRepository(AgrotourismContext db)
        {
            this.db = db;
        }

        public void AddRole(IdentityRole role)
        {
            db.Roles.Add(role);
        }

        public void AssignToRole(string id, string role)
        {
            RoleStore<IdentityRole> roleStore = new RoleStore<IdentityRole>(db);
            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(roleStore);

            UserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>(db);
            UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(userStore);
            userManager.AddToRole(id, role);
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public IQueryable<IdentityRole> GetRoles()
        {
            IQueryable<IdentityRole> roles = db.Roles.AsNoTracking();
            return roles;
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }


    }
}