using DomainModel.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Service.IService
{
    public interface IUserService : IDisposable
    {
        void AddRole(IdentityRole role);

        void AssignToRole(string userId, string role);

        IList<IdentityRole> GetRoles();

        void RemoveFromRole(string userId, string role);

        IList<User> GetUsers();

        User GetUserById(string id);

        IList<SelectListItem> GetNewRolesForUser(IList<IdentityUserRole> sserRoles, Dictionary<string, string> roles);

        int CountUsersForGivenRole(Dictionary<string, string> roles, string role);

        void UpdateUser(User user, string securityStamp);

        bool isUserEmployed(string userId);

        void RemoveUser(User user, string securityStamp);

        IList<string> GetAvaiableProfessions();

        void UpdateBaseDataUser(User user, string securityStamp);

        User GetOriginalUserValues(string id);

        IList<string> GetUserRoles(string id);

        void RemoveClientReservations(string userId);
    }
}
