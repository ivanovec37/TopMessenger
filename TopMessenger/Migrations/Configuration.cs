namespace TopMessenger.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using TopMessenger.Infrastructure.Services;
    using TopMessenger.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<TopMessenger.Data.AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(TopMessenger.Data.AppDbContext context)
        {
            
        }
    }
}
