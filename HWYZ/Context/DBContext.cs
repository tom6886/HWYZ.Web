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

        public DbSet<Store> Store { get; set; }

        public DbSet<Area> Area { get; set; }

        public DbSet<Product> Product { get; set; }

        public DbSet<StoreProduct> StoreProduct { get; set; }

        public DbSet<Doc> Doc { get; set; }

        public DbSet<Order> Order { get; set; }

        public DbSet<OrderItem> OrderItem { get; set; }

        public DbSet<Courier> Courier { get; set; }

        public DbSet<Location> Location { get; set; }

        public DbSet<Dictionary> Dictionary { get; set; }

        public DbSet<Notice> Notice { get; set; }

        public DbSet<Stock> Stock { get; set; }

        public DbSet<OfflineSell> OfflineSell { get; set; }
    }
}
