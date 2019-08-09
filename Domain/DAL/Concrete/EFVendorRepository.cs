using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Domain.DAL.Abstract;
using Domain.Entity;

namespace Domain.DAL.Concrete
{
   public class EFVendorRepository : IVendorRepository
    {
        private ViDBContext context;
        public EFVendorRepository(ViDBContext context)
        {
            this.context = context;
        }
        public IQueryable<Vendor> Vendors => context.Vendors;

        public void SaveVendor(Vendor vendor)
        {
            if (vendor.VendorID == 0)
            {
                context.Vendors.Add(vendor);
            }
            else
            {
                context.Entry(vendor).State = EntityState.Modified;
            }
            context.SaveChanges();
        }

        public void DeleteVendor(Vendor vendor)
        {
            try
            {
                context.Vendors.Remove(vendor);
                context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Vendor GetVendorByID(int vendorId)
        {
            return Vendors.FirstOrDefault(x => x.VendorID == vendorId);
        }
        
        public async Task SaveVendorAsync(Vendor vendor)
        {
            if (vendor.VendorID == 0)
            {
                context.Vendors.Add(vendor);
            }
            else
            {
                context.Entry(vendor).State = EntityState.Modified;
            }

            await context.SaveChangesAsync();
        }
      
        public async Task DeleteVendorAsync(Vendor vendor)
        {
            try
            {
                context.Vendors.Remove(vendor);
                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
      
        public async Task<Vendor> GetVendorByIDAsync(int vendorId)
        {
            return await Vendors.FirstOrDefaultAsync(x => x.VendorID == vendorId);
        }

    }
}
