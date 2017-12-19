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
        User GetOriginalUserValues(string id);
        void AssignToRole(string userId, string role);
        void UpdateUser(User user,string securityStamp);
        void UpdateBaseDataUser(User user, string securityStamp);
        void RemoveUser(User user,string securityStamp);
        void RemoveFromRole(string userId, string role);
        void AddRole(IdentityRole role);
        void SaveChanges();
    }
}
