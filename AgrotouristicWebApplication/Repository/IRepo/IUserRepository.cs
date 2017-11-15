using DomainModel.Models;
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
        IList<IdentityRole> GetRoles();
        IList<User> GetUsers();
        User GetUserById(string id);
        User GetOriginalValuesUser(string id);
        void AssignToRole(string userId, string role);
        void UpdateUser(User user,string securityStamp);
        void UpdateBaseDataUser(User user, string securityStamp);
        IList<SelectListItem> GetNewRolesForUser(IList<IdentityUserRole> userRoles, Dictionary<string, string> roles);
        IList<string> GetAvaiableProfessons();
        IList<string> GetUserRoles(string id);
        int GetNumberOfUsersForGivenRole(Dictionary<string, string> roles,string role);
        void RemoveUser(User user,string securityStamp);
        bool isUserEmployed(string userId);
        void RemoveFromRole(string userId, string role);
        void RemoveReservationsAssosiatedClient(string userId);
        void AddRole(IdentityRole role);
        void SaveChanges();
    }
}
