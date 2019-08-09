using System.Data.Entity.ModelConfiguration;
using Domain.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DAL.Configurations
{
    class VendorConfig : EntityTypeConfiguration<Vendor>
    {
        public VendorConfig()
        {
            HasKey(p => p.VendorID);            
            Property(p => p.VendorID).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(p => p.Name).IsRequired();            
            Property(p => p.CreationDate).HasColumnType("date");
            Property(p => p.UpdateDate).HasColumnType("date");
            HasMany(x => x.Products).WithRequired(x => x.Vendor).HasForeignKey(x=>x.VendorID);
        }
    }
}
