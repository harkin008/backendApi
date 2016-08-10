namespace test_webApiIdentity2.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using test_webApiIdentity2.Infrastructure;

    internal sealed class Configuration : DbMigrationsConfiguration<test_webApiIdentity2.Infrastructure.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(test_webApiIdentity2.Infrastructure.ApplicationDbContext context)
        {
            var manager = new UserManager<ApplicationUsers>(new UserStore<ApplicationUsers>(new ApplicationDbContext()));

            var user = new ApplicationUsers()
            {
                UserName = "SuperPowerUser",
                Email = "taiseer.joudeh@mymail.com",
                EmailConfirmed = true,
                FirstName = "Taiseer",
                LastName = "Joudeh",
                Level = 1,
                JoinDate = DateTime.Now.AddYears(-3)
            };

            manager.Create(user, "MySuperP@ssword!");



            //  This method will be called after migrating to the latest version.


        }
    }
}
