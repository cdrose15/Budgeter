namespace Budgeter.Migrations
{
    using Models.CodeFirst;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Budgeter.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Budgeter.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            context.CategoryLists.AddRange(
                new List<CategoryList>() {
                new CategoryList { Name = "Income" },
                new CategoryList { Name = "Automobile" },
                new CategoryList { Name = "Food" },
                new CategoryList { Name = "Clothing" },
                new CategoryList { Name = "Home" },
                new CategoryList { Name = "Utilities" },
                new CategoryList { Name = "Pet" },
                new CategoryList { Name = "Loans" }
                });
                
        }
    }
}
