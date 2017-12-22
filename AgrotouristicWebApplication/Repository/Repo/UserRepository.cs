using DomainModel.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Repository.IRepo;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Repository.Repository
{
    public class UserRepository: IdentityDbContext,IUserRepository
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

        public IList<IdentityRole> GetRoles()
        {
            IQueryable<IdentityRole> roles = db.Roles.AsNoTracking();

            return roles.ToList();
        }

        public new void SaveChanges()
        {
            db.SaveChanges();
        }

        public void RemoveFromRole(string userId, string role)
        {
            GetUserManager().RemoveFromRole(userId, role);
        }

        public IList<User> GetUsers()
        {
            IQueryable<User> users = db.ApplicationUsers.AsNoTracking();
            return users.ToList();
        }

        public User GetUserById(string id)
        {
            User user = db.ApplicationUsers.Find(id);
            return user;
        }

        public void UpdateUser(User user,string securityStamp)
        {
            if(!db.Entry(user).OriginalValues["SecurityStamp"].Equals(securityStamp))
            {
                throw new DbUpdateConcurrencyException(user.Id);
            }
            db.Entry(user).OriginalValues["SecurityStamp"] = securityStamp;
            GetUserManager().UpdateSecurityStamp(user.Id);
        }

        public void RemoveUser(User user,string securityStamp)
        {
            user = GetUserById(user.Id);
            if (!db.Entry(user).OriginalValues["SecurityStamp"].Equals(securityStamp))
            {
                throw new DbUpdateConcurrencyException(user.Id);
            }
            
            ICollection<IdentityUserLogin> logins = user.Logins;
            ICollection<IdentityUserRole> rolesForUser = user.Roles;
            Dictionary<string, string> roles = db.Roles.ToDictionary(x => x.Id, x => x.Name);

            logins.ToList().ForEach(item => GetUserManager().RemoveLogin(user.Id, new UserLoginInfo(item.LoginProvider, item.ProviderKey)));

            rolesForUser.ToList().ForEach(item => RemoveFromRole(user.Id,roles[item.RoleId]));
            db.Entry(user).State = EntityState.Deleted;
        }

        public void UpdateBaseDataUser(User user,string securityStamp)
        {
            if (!db.Entry(user).OriginalValues["SecurityStamp"].Equals(securityStamp))
            {
                throw new DbUpdateConcurrencyException(user.Id);
            }
            db.Entry(user).OriginalValues["SecurityStamp"] = securityStamp;
            GetUserManager().UpdateSecurityStamp(user.Id);
        }

        public User GetOriginalUserValues(string id)
        {
            User user = GetUserById(id);
            User orignal = new User();
            orignal.Id = db.Entry(user).OriginalValues["Id"].ToString();
            orignal.PasswordHash = db.Entry(user).OriginalValues["PasswordHash"].ToString();
            orignal.SecurityStamp = db.Entry(user).OriginalValues["SecurityStamp"].ToString();
            orignal.BirthDate = DateTime.Parse(db.Entry(user).OriginalValues["BirthDate"].ToString());
            orignal.Email = db.Entry(user).OriginalValues["Email"].ToString();
            orignal.EmailConfirmed = Boolean.Parse(db.Entry(user).OriginalValues["EmailConfirmed"].ToString());
            orignal.HireDate = DateTime.Parse(db.Entry(user).OriginalValues["HireDate"].ToString() != null ? db.Entry(user).OriginalValues["HireDate"].ToString() : null);
            orignal.Name = db.Entry(user).OriginalValues["Name"].ToString();
            orignal.Surname = db.Entry(user).OriginalValues["Surname"].ToString();
            orignal.PhoneNumber = db.Entry(user).OriginalValues["PhoneNumber"].ToString();
            orignal.Profession = db.Entry(user).OriginalValues["Profession"].ToString();
            orignal.Salary = Decimal.Parse(db.Entry(user).OriginalValues["Salary"].ToString());
            orignal.UserName = db.Entry(user).OriginalValues["UserName"].ToString();
            return orignal;
        }

    }
}