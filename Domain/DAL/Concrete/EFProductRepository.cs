using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Domain.DAL.Abstract;
using Domain.Entity;
using Domain.ExtensionMethods;

namespace Domain.DAL.Concrete
{
    public class EFProductRepository : IProductRepository
    {
        private ViDBContext context;

        public EFProductRepository(ViDBContext context)
        {
            this.context = context;            
        }

        public IQueryable<Product> Products => context.Products;

        public void SaveProduct(Product product)
        {
            
            Product productOrigin = context.Products.FirstOrDefault(x => x.Code == product.Code);

            if(productOrigin==null)
            {
                //
                product.CreationDate = DateTime.Now.Date;
                context.Products.Add(product);
                context.SaveChanges();
            }
            else
            {
              //  context.Entry(productOrigin).State = EntityState.Detached;
                //context.Products.Attach(productOrigin);
                if (!productOrigin.Equals(product))
                {
                    productOrigin.CloneProduct(product);
                    context.Entry(productOrigin).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }            
        }

        public void DeleteProduct(Product product)
        {
            try
            {
                context.Products.Remove(product);
                context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Product GetProductByCode(int code)
        {
            return Products.FirstOrDefault(x => x.Code == code);
        }
        public Product GetProductByID(int productId)
        {
            return Products.FirstOrDefault(x => x.ProductID == productId);
        }

        public async Task SaveProductAsync(Product product)
        {
            Product productOrigin = await context.Products.FirstOrDefaultAsync(x => x.Code == product.Code);
            if (productOrigin == null)
            {
                product.CreationDate = DateTime.Now.Date;
                context.Products.Add(product);
                await context.SaveChangesAsync();
            }
            else
            {
                if (!productOrigin.Equals(product))
                {
                    productOrigin.CloneProduct(product);
                    context.Entry(productOrigin).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                }
            }
        }
        public async Task DeleteProductAsync(Product product)
        {
            try
            {
                context.Products.Remove(product);
                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Product> GetProductByCodeAsync(int code)
        {
            return await context.Products.FirstOrDefaultAsync(x => x.Code == code);
        }

        public async Task<Product> GetProductByIDAsync(int productId)
        {
            return await context.Products.FirstOrDefaultAsync(x => x.ProductID == productId);
        }

        public void SaveProductRange(IEnumerable<Product> products)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    List<Product> updateList = new List<Product>();

                    foreach (Product product in products)
                    {
                        Product productOrigin = context.Products.FirstOrDefault(x => x.Code == product.Code);
                        if (productOrigin != null)
                        {
                            productOrigin.CloneProduct(product);
                            context.Entry(productOrigin).State = EntityState.Modified;
                            updateList.Add(product);
                        }                        
                    }
                    context.SaveChanges();
                    List<Product> insertList = new List<Product>();
                    insertList.AddRange(products.Except(updateList).ToList());
                    
                    context.Configuration.AutoDetectChangesEnabled = false;
                    context.Configuration.ValidateOnSaveEnabled = false;

                    context.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s));

                    foreach (Product product in insertList)
                    {
                        product.CreationDate = DateTime.Now.Date;
                        context.Entry(product).State = EntityState.Added;
                    }                   

                    context.SaveChanges();
                    context.Configuration.AutoDetectChangesEnabled = true;
                    context.Configuration.ValidateOnSaveEnabled = true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Ошибка вставки: " + ex.InnerException.Message);
                }
            }
        }

        public void DeleteProductRange(IEnumerable<Product> products)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    context.Products.RemoveRange(products);
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Ошибка удаления: " + ex.InnerException.Message);
                }
            }
        }

        public async Task SaveProductRangeAsync(IEnumerable<Product> products)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    List<Product> updateList = new List<Product>();

                    foreach (Product product in products)
                    {
                        Product productOrigin = await context.Products.FirstOrDefaultAsync(x => x.Code == product.Code);
                        if (productOrigin != null)
                        {
                            productOrigin.CloneProduct(product);
                            context.Entry(productOrigin).State = EntityState.Modified;
                            updateList.Add(product);
                        }
                    }

                    IEnumerable<Product> insertList = products.Except(updateList);

                    foreach (Product product in insertList)
                    {
                        context.Configuration.AutoDetectChangesEnabled = false;
                        context.Products.AddRange(products);
                        context.Configuration.AutoDetectChangesEnabled = true;
                    }

                   await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Ошибка вставки: " + ex.InnerException.Message);
                }
            }
        }

        public async Task DeleteProductRangeAsync(IEnumerable<Product> products)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    context.Products.RemoveRange(products);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Ошибка удаления: " + ex.InnerException.Message);
                }
            }
        }

        public async Task<IEnumerable<Product>> GetProductListByVendor(Vendor vendor)
        {            
            return await Products.Where(x => x.VendorID == vendor.VendorID).AsNoTracking().ToListAsync();
        }

        public async Task ChangeFavoriteStatus(Product product)
        {
            //productOrigin.CloneProduct(product);
            product.IsFavorite = !product.IsFavorite;
            context.Entry(product).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
    }
}
