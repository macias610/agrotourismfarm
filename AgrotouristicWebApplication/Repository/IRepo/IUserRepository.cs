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
        User GetOriginalValuesUser(string id);
        void AssignToRole(string userId, string role);
        void UpdateUser(User user,string securityStamp);
        void UpdateBaseDataUser(User user);
        List<SelectListItem> GetNewRolesForUser(List<IdentityUserRole> UserRoles, Dictionary<string, string> Roles);
        List<string> GetRolesForUser(ICollection<IdentityUserRole> userRoles);
        List<string> GetAvaiableProfessons();
        ICollection<IdentityUserRole> GetUserRoles(string id);
        int GetNumberOfUsersForGivenRole(Dictionary<string, string> Roles,string role);
        void RemoveUser(User user,string securityStamp);
        bool isUserEmployed(string userId);
        void RemoveFromRole(string userId, string role);
        void AddRole(IdentityRole role);
        void SaveChanges();
    }
}
