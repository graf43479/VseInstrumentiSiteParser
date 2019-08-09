using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entity;

namespace Domain.DAL.Abstract
{
    public interface IProductRepository
    {
        IQueryable<Product> Products { get; }
        void SaveProduct(Product product);
        void DeleteProduct(Product product);
        void DeleteProductRange(IEnumerable<Product> products);
        void SaveProductRange(IEnumerable<Product> products);
        Product GetProductByID(int productId);
        Product GetProductByCode(int code);
        Task SaveProductAsync(Product product);
        Task DeleteProductAsync(Product product);
        Task DeleteProductRangeAsync(IEnumerable<Product> products);
        Task SaveProductRangeAsync(IEnumerable<Product> products);
        Task<Product> GetProductByIDAsync(int productId);
        Task<Product> GetProductByCodeAsync(int code);
        Task<IEnumerable<Product>> GetProductListByVendor(Vendor vendor);
        Task ChangeFavoriteStatus(Product product);
    }
}
