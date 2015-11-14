using HWYZ.Models;
using System.Data.Entity;

namespace HWYZ.Context
{
    public class DBContext : DbContext
    {
        public DBContext()
            : base("Name=DBContext")
        {
            base.Configuration.ProxyCreationEnabled = false;
            base.Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<Guser> Guser { get; set; }

        public DbSet<GuserRole> GuserRole { get; set; }
    }
}
