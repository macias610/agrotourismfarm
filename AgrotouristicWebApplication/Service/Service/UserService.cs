using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Mvc;
using Repository.IRepo;
using DomainModel.Models;
using Repository.Models;
using Repository.Repository;

namespace Service
{
    public class UserService : IUserService 
    {
        private readonly IUserRepository userRepository = null;

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public UserService()
        {
            this.userRepository = new UserRepository(new AgrotourismContext());
        }

        public void AddRole(IdentityRole role)
        {
            this.userRepository.AddRole(role);
            this.userRepository.SaveChanges();
        }

        public void AssignToRole(string userId, string role)
        {
            this.userRepository.AssignToRole(userId, role);
            this.userRepository.SaveChanges();
        }

        public IList<string> GetAvaiableProfessons()
        {
            IList<string> avaiableProfessions = this.userRepository.GetAvaiableProfessons();

            return avaiableProfessions;
        }

        public IList<SelectListItem> GetNewRolesForUser(IList<IdentityUserRole> userRoles, Dictionary<string, string> roles)
        {
            IList<SelectListItem> newRoles = this.userRepository.GetNewRolesForUser(userRoles, roles);
            return newRoles;
        }

        public int GetNumberOfUsersForGivenRole(Dictionary<string, string> roles, string role)
        {
            int numbers = this.userRepository.GetNumberOfUsersForGivenRole(roles, role);
            return numbers;
        }

        public User GetOriginalValuesUser(string id)
        {
            User user = this.userRepository.GetOriginalValuesUser(id);

            return user;
        }

        public IList<IdentityRole> GetRoles()
        {
            IList<IdentityRole> roles = this.userRepository.GetRoles();
            return roles;
        }

        public User GetUserById(string id)
        {
            User user = this.userRepository.GetUserById(id);
            return user;
        }

        public IList<string> GetUserRoles(string id)
        {
            IList<string> userRoles = this.userRepository.GetUserRoles(id);

            return userRoles;
        }

        public IList<User> GetUsers()
        {
            IList<User> users = this.userRepository.GetUsers();
            return users;
        }

        public bool isUserEmployed(string userId)
        {
            bool isEmployed = this.userRepository.isUserEmployed(userId);
            return isEmployed;
        }

        public void RemoveFromRole(string userId, string role)
        {
            this.userRepository.RemoveFromRole(userId, role);
            this.userRepository.SaveChanges();
        }

        public void RemoveReservationsAssosiatedClient(string userId)
        {
            this.userRepository.RemoveReservationsAssosiatedClient(userId);
        }

        public void RemoveUser(User user, string securityStamp)
        {
            this.userRepository.RemoveUser(user, securityStamp);
            this.userRepository.SaveChanges();
        }

        public void UpdateBaseDataUser(User user, string securityStamp)
        {
            this.userRepository.UpdateBaseDataUser(user, securityStamp);
            this.userRepository.SaveChanges();
        }

        public void UpdateUser(User user, string securityStamp)
        {
            this.userRepository.UpdateUser(user, securityStamp);
            this.userRepository.SaveChanges();
        }

        public void Dispose()
        {
            userRepository.Dispose();
        }
    }
}
