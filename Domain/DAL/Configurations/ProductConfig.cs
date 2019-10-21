using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Domain.Entity;

namespace Domain.DAL.Configurations
{
    class ProductConfig : EntityTypeConfiguration<Product>
    {
        public ProductConfig()
        {
            HasKey(p => p.ProductID);
            Property(p => p.ProductID).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(p => p.Code).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(p => p.Name).IsRequired();
            Property(p => p.CurrentPrice).IsRequired();
            Property(p => p.Url).IsOptional();
            Property(p => p.CreationDate).HasColumnType("date");
            Property(p => p.UpdateDate).HasColumnType("date");
            HasMany(x => x.Prices).WithRequired(x => x.Product).HasForeignKey(x=>x.ProductID);    
        }
    }
}
