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
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<House> Houses { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Reservation_House> Reservation_House { get; set; }
        public DbSet<Reservation_House_Participant> Reservation_House_Participant { get; set; }
        public DbSet<Attraction_Reservation> Attraction_Reservation { get; set; }
        public DbSet<Attraction_Reservation_Worker> Attraction_Reservation_Worker { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Entity<Client>().ToTable("Client");
            modelBuilder.Entity<Worker>().ToTable("Worker");
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<Reservation>().HasRequired(x => x.Client)
                                              .WithMany(x => x.Reservations)
                                              .HasForeignKey(x => x.ClientId)
                                              .WillCascadeOnDelete(true);

            modelBuilder.Entity<Reservation_House>().HasRequired(x => x.Meal)
                                              .WithMany(x => x.Reservation_House)
                                              .HasForeignKey(x => x.MealId)
                                              .WillCascadeOnDelete(true);

        }

    }
}