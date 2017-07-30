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
    public interface IRoleRepository: IDisposable
    {
        IQueryable<IdentityRole> GetRoles();
        IQueryable<User> GetUsers();
        IdentityUser GetUserById(string id);
        void AssignToRole(string userId, string role);
        List<SelectListItem> GetNewRolesForUser(List<IdentityUserRole> UserRoles, Dictionary<string, string> Roles);
        void RemoveFromRole(User user, string role);
        void AddRole(IdentityRole role);
        void SaveChanges();
    }
}
