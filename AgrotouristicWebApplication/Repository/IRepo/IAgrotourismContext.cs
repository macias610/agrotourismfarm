using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Infrastructure;
using System.Web.Security;
using System.Web.Mvc;
using DomainModel.Models;

namespace Repository.IRepo
{
    public interface IAgrotourismContext
    {
        DbSet<Attraction> Attractions { get; set; }
        DbSet<Reservation> Reservations { get; set; }
        DbSet<User> ApplicationUsers { get; set; }
        DbSet<House> Houses { get; set; }
        DbSet<HouseType> HouseTypes { get; set; }
        DbSet<Meal> Meals { get; set; }
        DbSet<Participant> Participants { get; set; }
        DbSet<ReservationHouse> Reservation_Houses { get; set; }
        DbSet<ReservationHistory> Reservations_History { get; set; }
        DbSet<AttractionReservation> Attractions_Reservations { get; set; }

        void Dispose();

        DbSet<AttractionReservationWorker> Attractions_Reservations_Workers { get; set; }

        DbEntityEntry Entry(object entity);

        int SaveChanges();
    }
}
