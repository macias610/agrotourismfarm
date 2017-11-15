using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System;
using System.Data.Entity.ModelConfiguration.Conventions;
using Repository.IRepo;

namespace Repository.Models
{

    public class AgrotourismContext : IdentityDbContext,IAgrotourismContext
    {
        public AgrotourismContext()
            : base("DefaultConnection")
        {
        }

        public static AgrotourismContext Create()
        {
            return new AgrotourismContext();
        }

        public DbSet<Attraction> Attractions { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<User> ApplicationUsers { get; set; }
        public DbSet<House> Houses { get; set; }
        public DbSet<HouseType> HouseTypes { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Reservation_House> Reservation_Houses { get; set; }
        public DbSet<Reservation_History> Reservations_History { get; set; }
        public DbSet<Attraction_Reservation> Attractions_Reservations { get; set; }
        public DbSet<Attraction_Reservation_Worker> Attractions_Reservations_Workers { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
                                            

            modelBuilder.Entity<Reservation_House>().HasRequired(x => x.Reservation)
                                            .WithMany(x => x.Reservation_House)
                                            .HasForeignKey(x => x.ReservationId)
                                            .WillCascadeOnDelete(true);

            modelBuilder.Entity<Participant>().HasRequired(x => x.Reservation_House)
                                            .WithMany(x => x.Participant)
                                            .HasForeignKey(x => x.Reservation_HouseId)
                                            .WillCascadeOnDelete(true);

            modelBuilder.Entity<Attraction_Reservation>().HasRequired(x => x.Reservation)
                                            .WithMany(x => x.Attraction_Reservation)
                                            .HasForeignKey(x => x.ReservationId)
                                            .WillCascadeOnDelete(true);

            modelBuilder.Entity<Attraction_Reservation_Worker>().HasRequired(x => x.Attraction_Reservation)
                                            .WithMany(x => x.Attraction_Reservation_Worker)
                                            .HasForeignKey(x => x.Attraction_ReservationId)
                                            .WillCascadeOnDelete(true);


        }

    }
}