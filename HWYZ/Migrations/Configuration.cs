namespace HWYZ.Migrations
{
    using HWYZ.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Utils;

    internal sealed class Configuration : DbMigrationsConfiguration<HWYZ.Context.DBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(HWYZ.Context.DBContext context)
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
            //Guser user = new Guser()
            //{
            //    Account = "admin",
            //    PassWord = StringUtil.Md5Encrypt("111111"),
            //    CardNumber = "123456",
            //    Name = "π‹¿Ì‘±",
            //    Sex = Sex.male,
            //    Tel = "123456"
            //};


            //context.Guser.Add(user);
        }
    }
}
