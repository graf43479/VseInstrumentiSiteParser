using System.Data.Entity.Migrations;

namespace Domain.DAL
{
    sealed class Configuration : DbMigrationsConfiguration<ViDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(ViDBContext context)
        {
            
        }
    }
}
