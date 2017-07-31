using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Repository.IRepo;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Repository.Repository
{
    public class UserRepository: IUserRepository
    {
        private readonly AgrotourismContext db;

        public UserRepository(AgrotourismContext db)
        {
            this.db = db;
        }

        public void AddRole(IdentityRole role)
        {
            db.Roles.Add(role);
        }

        private UserManager<User> GetUserManager()
        {
            RoleStore<IdentityRole> roleStore = new RoleStore<IdentityRole>(db);
            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(roleStore);

            UserStore<User> userStore = new UserStore<User>(db);
            return new UserManager<User>(userStore);
        }

        public void AssignToRole(string userId, string role)
        {
            GetUserManager().AddToRole(userId, role);
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

        public void RemoveFromRole(string userId, string role)
        {
            GetUserManager().RemoveFromRole(userId, role);
        }

        public IQueryable<User> GetUsers()
        {
            IQueryable<User> users = db.ApplicationUsers.AsNoTracking();
            return users;
        }

        public User GetUserById(string id)
        {
            User user = db.ApplicationUsers.Find(id);
            return user;
        }

        public List<SelectListItem> GetNewRolesForUser(List<IdentityUserRole> UserRoles, Dictionary<string, string> Roles)
        {
            Dictionary<int,string> avaiableRoles = new Dictionary<int, string>();
            List<string> IdUserRoles = new List<string>();
            UserRoles.ForEach(item => IdUserRoles.Add(item.RoleId));

            int index = 0;

            foreach (KeyValuePair<string, string> Role in Roles)
            {
                if (!IdUserRoles.Contains(Role.Key))
                {
                    avaiableRoles.Add(index,Role.Value);
                    index++;
                }
            }
            List<SelectListItem> selectList = avaiableRoles.Select(avaiableRole => new SelectListItem { Value = avaiableRole.Key.ToString(), Text = avaiableRole.Value }).ToList();
            return selectList;
        }

        public int GetNumberOfUsersForGivenRole(Dictionary<string, string> Roles,string role)
        {
            List<string> userRoles = new List<string>();
            int numberOfUsers = 0;

            foreach(IdentityUser user in db.Users)
            {
                List <IdentityUserRole> list= user.Roles.ToList();
                list.ForEach(x => userRoles.Add(x.RoleId));
            }
   
            string roleId = Roles[role];

            foreach(string userRole in userRoles)
            {
                if(userRole.Equals(roleId))
                {
                    numberOfUsers++;
                }
            }
            return numberOfUsers;
        }

        public void UpdateUser(User user)
        {
            db.Entry(user).State = EntityState.Modified;
        }
    }
}