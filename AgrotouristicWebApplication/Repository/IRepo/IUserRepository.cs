using Microsoft.AspNet.Identity.EntityFramework;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Repository.IRepo
{
    public interface IUserRepository: IDisposable
    {
        IQueryable<IdentityRole> GetRoles();
        IQueryable<User> GetUsers();
        User GetUserById(string id);
        void AssignToRole(string userId, string role);
        void UpdateUser(User user);
        List<SelectListItem> GetNewRolesForUser(List<IdentityUserRole> UserRoles, Dictionary<string, string> Roles);
        int GetNumberOfUsersForGivenRole(Dictionary<string, string> Roles,string role);
        void RemoveUser(string id);
        bool isUserEmployed(string userId);
        void RemoveFromRole(string userId, string role);
        void AddRole(IdentityRole role);
        void SaveChanges();
    }
}
