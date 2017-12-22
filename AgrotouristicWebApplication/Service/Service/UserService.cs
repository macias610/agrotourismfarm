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
using System.Transactions;

namespace Service
{
    public class UserService : IUserService 
    {
        private readonly IUserRepository userRepository = null;
        private readonly IAttractionRepository attractionRepository = null;
        private readonly IReservationRepository reservationRepository = null;

        public UserService(IUserRepository userRepository, IAttractionRepository attractionRepository, IReservationRepository reservationRepository)
        {
            this.userRepository = userRepository;
            this.attractionRepository = attractionRepository;
            this.reservationRepository = reservationRepository;
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

        public IList<string> GetAvaiableProfessions()
        {
            IList<Attraction> attractions = this.attractionRepository.GetAttractions();
            IList<string> professions = attractions.Select(item => item.Name).ToList();
            professions.Add("Administrator");
            professions.Add("-");
            return professions;
        }

        public IList<SelectListItem> GetNewRolesForUser(IList<IdentityUserRole> userRoles, Dictionary<string, string> roles)
        {
            Dictionary<int, string> avaiableRoles = new Dictionary<int, string>();
            List<string> idUserRoles = new List<string>();
            userRoles.ToList().ForEach(item => idUserRoles.Add(item.RoleId));

            int index = 0;

            foreach (KeyValuePair<string, string> role in roles)
            {
                if (!idUserRoles.Contains(role.Key))
                {
                    avaiableRoles.Add(index, role.Value);
                    index++;
                }
            }
            List<SelectListItem> selectList = avaiableRoles
                                                .Select(avaiableRole => new SelectListItem { Value = avaiableRole.Key.ToString(), Text = avaiableRole.Value })
                                                .ToList();
            return selectList;
        }

        public int CountUsersForGivenRole(Dictionary<string, string> roles, string role)
        {
            IList<User> users = this.userRepository.GetUsers();
            List<string> userRoles = new List<string>();
            int numberOfUsers = 0;

            foreach (IdentityUser user in users)
            {
                List<IdentityUserRole> list = user.Roles.ToList();
                list.ForEach(x => userRoles.Add(x.RoleId));
            }

            string roleId = roles[role];

            foreach (string userRole in userRoles)
            {
                if (userRole.Equals(roleId))
                {
                    numberOfUsers++;
                }
            }
            return numberOfUsers;
        }

        public User GetOriginalUserValues(string id)
        {
            User user = this.userRepository.GetOriginalUserValues(id);
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
            User user = GetUserById(id);
            Dictionary<string, string> roles = GetRoles().ToDictionary(x => x.Id, x => x.Name);
            List<string> userRoles = new List<string>();
            foreach (IdentityUserRole userRole in user.Roles)
            {
                userRoles.Add(roles[userRole.RoleId]);
            }
            return userRoles;
        }

        public IList<User> GetUsers()
        {
            IList<User> users = this.userRepository.GetUsers();
            return users;
        }

        public bool isUserEmployed(string userId)
        {
            Dictionary<string, string> roles = this.userRepository.GetRoles().ToDictionary(x => x.Name, x => x.Id);
            roles.Remove("Klient");
            List<string> userRoles = new List<string>();
            this.userRepository.GetUserById(userId).Roles
                .ToList()
                .ForEach(item => userRoles.Add(item.RoleId));

            foreach (string userRoleId in userRoles)
            {
                if (roles.ContainsValue(userRoleId))
                {
                    return true;
                }
            }
            return false;
        }

        public void RemoveFromRole(string userId, string role)
        {
            this.userRepository.RemoveFromRole(userId, role);
            this.userRepository.SaveChanges();
        }

        public void RemoveClientReservations(string userId)
        {
            IList<Reservation> clientReservations = this.reservationRepository
                                                            .GetReservations()
                                                            .Where(item => item.ClientId.Equals(userId))
                                                            .ToList();
            using (TransactionScope scope = new TransactionScope())
            {
                foreach(Reservation reservation in clientReservations)
                {
                    this.reservationRepository.RemoveReservation(reservation);
                    this.reservationRepository.SaveChanges();
                }
                scope.Complete();
            }
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
