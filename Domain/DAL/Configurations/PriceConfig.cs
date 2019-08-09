using System.Data.Entity.ModelConfiguration;
using Domain.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DAL.Configurations
{
    class PriceConfig : EntityTypeConfiguration<Price>
    {
        public PriceConfig() 
        {
            ToTable("Price");
            HasKey(p => p.PriceID);
            Property(p => p.PriceID).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(p => p.PriceValue).IsRequired();            
        }
    }
}
