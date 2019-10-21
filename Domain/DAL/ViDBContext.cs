using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Domain.DAL.Configurations;
using Domain.Entity;

namespace Domain.DAL
{
    public class ViDBContext : DbContext
    {
        public ViDBContext() : base ("DbConnection")
        {         
        }

        static ViDBContext()
        {
            //Database.SetInitializer<ViDBContext>(new ViDBInitializer());
            Database.SetInitializer<ViDBContext>(null);
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Statistic> Statistics { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Configurations.Add(new ProductConfig());
            modelBuilder.Configurations.Add(new VendorConfig());
            modelBuilder.Configurations.Add(new StatisticConfig());
            modelBuilder.Configurations.Add(new PriceConfig());
        }
    }
}

