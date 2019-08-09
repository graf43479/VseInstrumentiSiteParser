using System.Linq;
using System.Threading.Tasks;
using Domain.Entity;

namespace Domain.DAL.Abstract
{
    public interface IVendorRepository
    {
        IQueryable<Vendor> Vendors { get; }
        void SaveVendor(Vendor vendor);
        void DeleteVendor(Vendor vendor);
        Vendor GetVendorByID(int vendorId);    
        Task SaveVendorAsync(Vendor vendor);
        Task DeleteVendorAsync(Vendor vendor);
        Task<Vendor> GetVendorByIDAsync(int vendorId);
    }
}
