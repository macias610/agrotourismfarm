using Microsoft.AspNet.Identity.EntityFramework;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepo
{
    public interface IRoleRepository: IDisposable
    {
        IQueryable<IdentityRole> GetRoles();
        void AssignToRole(string id, string role);
        void AddRole(IdentityRole role);
        void SaveChanges();
    }
}
