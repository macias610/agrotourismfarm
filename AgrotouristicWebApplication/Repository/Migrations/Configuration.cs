namespace Repository.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Repository.Models.AgrotourismContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(AgrotourismContext context)
        {
            SeedRoles(context);
            SeedUsers(context);

        }

        private void SeedRoles(AgrotourismContext context)
        {
            RoleManager<IdentityRole> roleManager = new RoleManager<Microsoft.AspNet.Identity.EntityFramework.IdentityRole>(new RoleStore<IdentityRole>());

            if (!roleManager.RoleExists("Admin"))
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);
            }
        }

        private void SeedUsers(AgrotourismContext context)
        {
            UserStore<Worker> store = new UserStore<Worker>(context);
            UserManager<Worker> manager = new UserManager<Worker>(store);

            if (!context.Users.Any(u => u.UserName == "Admin@gmail.com"))
            {
                Worker user = new Worker { UserName = "Admin@gmail.com", Name = "Adam", Surname = "Kowalski", BirthDate = DateTime.Now, HireDate=DateTime.Now, Profession="Admin",Salary= 2300 };
                var adminresult = manager.Create(user, "Password6#");

                if (adminresult.Succeeded)
                {
                    manager.AddToRole(user.Id, "Admin");
                }
            }

        }
    }
}
