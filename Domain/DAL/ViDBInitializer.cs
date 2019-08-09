using System;
using System.Collections.Generic;
using System.Data.Entity;
using Domain.Entity;

namespace Domain.DAL
{
    class ViDBInitializer : DropCreateDatabaseAlways<ViDBContext> //MigrateDatabaseToLatestVersion<ViDBContext, Configuration> //
    {
        protected override void Seed(ViDBContext context)
        {
            var vendors = new List<Vendor>
            {
                new Vendor { Name = "Crown", SubUrl = "crown", CreationDate=DateTime.Now, UpdateDate=DateTime.Now },
                new Vendor {  Name = "Ryobi", SubUrl = "ryobi", CreationDate=DateTime.Now, UpdateDate=DateTime.Now }
            };

            vendors.ForEach(x => context.Vendors.Add(x));
            context.SaveChanges();
        }
    }
}
