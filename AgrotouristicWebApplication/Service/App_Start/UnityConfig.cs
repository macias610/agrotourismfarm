using System;
using Microsoft.Practices.Unity;
using Repository.IRepo;
using Repository.Repository;
using Repository.Repo;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using Microsoft.Owin.Security;
using System.Web;
using Service.IService;
using Repository.Models;
using Service.Service;
using DomainModel.Models;

namespace Service.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            container.RegisterType(typeof(UserManager<>),
                        new InjectionConstructor(typeof(IUserStore<>)));
            container.RegisterType<Microsoft.AspNet.Identity.IUser>(new InjectionFactory(c => c.Resolve<Microsoft.AspNet.Identity.IUser>()));
            container.RegisterType(typeof(IUserStore<>), typeof(UserStore<>));
            container.RegisterType<IdentityUser, User>(new PerRequestLifetimeManager());
            container.RegisterType<DbContext, AgrotourismContext>(new ContainerControlledLifetimeManager());

            container.RegisterType<UserManager<User>>(new PerRequestLifetimeManager());
            container.RegisterType<IUserStore<User>, UserStore<User>>(new PerRequestLifetimeManager());
            container.RegisterType<DbContext, AgrotourismContext>(new HierarchicalLifetimeManager());

            container.RegisterType<IAuthenticationManager>(
    new InjectionFactory(
        o => System.Web.HttpContext.Current.GetOwinContext().Authentication
    )
);


          
            
            container.RegisterType<IAccountService, AccountService>(new PerRequestLifetimeManager());
            container.RegisterType<IAttractionService, AttractionService>(new PerRequestLifetimeManager());
            container.RegisterType<IHomeService, HomeService>(new PerRequestLifetimeManager());
            container.RegisterType<IHouseService, HouseService>(new PerRequestLifetimeManager());
            container.RegisterType<IMealService, MealService>(new PerRequestLifetimeManager());
            container.RegisterType<IDetailsReservationService, DetailsReservationService>(new PerRequestLifetimeManager());
            container.RegisterType<IReservationService, ReservationService>(new PerRequestLifetimeManager());
            container.RegisterType<IReservedAttractionsService, ReservedAttractionsService>(new PerRequestLifetimeManager());
            container.RegisterType<IHouseTypeService, HouseTypeService>(new PerRequestLifetimeManager());
            container.RegisterType<IAttractionReservationService, AttractionReservationService>(new PerRequestLifetimeManager());
            container.RegisterType<IMealReservationService, MealReservationService>(new PerRequestLifetimeManager());
            container.RegisterType<IHouseReservationService, HouseReservationService>(new PerRequestLifetimeManager());
            container.RegisterType<IParticipantReservationService, ParticipantReservationService>(new PerRequestLifetimeManager());
            container.RegisterType<IUserService, UserService>(new PerRequestLifetimeManager());



            container.RegisterType<IUserRepository, UserRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IHouseRepository, HouseRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IMealRepository, MealRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IAttractionRepository, AttractionRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IReservationRepository, ReservationRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IReservedAttractionsRepository, ReservedAttractionsRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IHomeRepository, HomeRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IAttractionReservationRepository, AttractionReservationRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IReservationHouseRepository, ReservationHouseRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IHouseTypeRepository, HouseTypeRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IParticipantRepository, ParticipantRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IAgrotourismContext, AgrotourismContext>(new PerRequestLifetimeManager());
        }
    }
}
