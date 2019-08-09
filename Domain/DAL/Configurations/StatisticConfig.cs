using System.Data.Entity.ModelConfiguration;
using Domain.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DAL.Configurations
{
    class StatisticConfig : EntityTypeConfiguration<Statistic>
    {
        public StatisticConfig()
        {
            ToTable("Statistic");
            HasKey(p => p.StatisticID);
            Property(p => p.StatisticID).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(p => p.CreationDate).IsRequired();
            Property(p => p.CreationDate).HasColumnType("datetime2");     
            HasMany(x => x.Prices).WithRequired(x => x.Statistic).HasForeignKey(x => x.StatisticID);
        }
    }
}
