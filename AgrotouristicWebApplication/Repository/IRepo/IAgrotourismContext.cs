﻿using Repository.Models;
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

namespace Repository.IRepo
{
    public interface IAgrotourismContext
    {
        DbSet<Attraction> Attractions { get; set; }
        DbSet<Reservation> Reservations { get; set; }
        DbSet<ApplicationUser> ApplicationUsers { get; set; }
        DbSet<House> Houses { get; set; }
        DbSet<Meal> Meals { get; set; }
        DbSet<Participant> Participants { get; set; }
        DbSet<Reservation_House> Reservation_House { get; set; }
        DbSet<Reservation_House_Participant> Reservation_House_Participant { get; set; }
        DbSet<Attraction_Reservation> Attraction_Reservation { get; set; }
        DbSet<Attraction_Reservation_Worker> Attraction_Reservation_Worker { get; set; }

        DbEntityEntry Entry(object entity);

        int SaveChanges();
    }
}