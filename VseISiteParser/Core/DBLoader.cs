using System;
using System.Collections.Generic;
using Domain.DAL;
using Domain.DAL.Abstract;
using Domain.DAL.Concrete;
using System.Linq;
using Domain.Entity;
using System.Data.Entity;
using System.Threading.Tasks;
using Domain.ExtensionMethods;
using Domain;
using VseISiteParser.Interfaces;

namespace VseISiteParser.Core
{
   public class DBLoader : IDBLoader
    {        
       private IStatisticRepository statisticRepository; // = new EFStatisticRepository(db);
       private IVendorRepository vendorRepository; // = new EFVendorRepository(db);
       private IPriceRepository priceRepository; // = new EFPriceRepository(db);
       private IProductRepository productRepository; // = new EFProductRepository(db);

        public DBLoader(ViDBContext db)
        {
            statisticRepository = new EFStatisticRepository(db);
            vendorRepository = new EFVendorRepository(db);
            priceRepository = new EFPriceRepository(db);
            productRepository = new EFProductRepository(db);          
        }

        string Status = "Success"; //меняется на false в случае проблем
               
        /// <summary>
        /// Получение списка производителей
        /// </summary>
        /// <returns></returns>
        public async Task<List<Vendor>>  GetVendorsAsync()
        {
            return await vendorRepository.Vendors.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Сохранение списка в БД поштучно
        /// </summary>
        /// <param name="products">список товаров</param>
       public void LoadProductList(List<Product> products)
        {
            try
            {
                foreach (var item in products)
                {
                    Task t = productRepository.SaveProductAsync(item);
                    t.Wait();
                }
            }
            catch (Exception)
            {
                Status = "Falled at ProductList upload";
            }
        }

        /// <summary>
        /// Получение информации о слепках цен
        /// </summary>
        /// <returns>список слепков</returns>
       public IEnumerable<Statistic> GetStatistic()
        {
            return statisticRepository.Statistics;
        }

        /// <summary>
        /// Инициирует создание контрольной точки и слепок цен на товары от сегодняшней даты
        /// </summary>
        /// <returns></returns>
        public async Task MakePriceSnapshotAsync()
        {
            Statistic statistic = new Statistic()
            {
                CreationDate = DateTime.Now.Date
            };

            statisticRepository.SaveStatistic(statistic);          

            try
            {
                int startFrom = 0;
                int blockSize = 1000;

                IQueryable<Product> total = productRepository.Products.OrderBy(x => x.Code); //.Where(x => x.UpdateDate.Date == DateTime.Now.Date);

                for (int i = startFrom; i < total.Count() + blockSize; i += blockSize)
                {
                    if (total == null) break;
                    List<Price> prices = (await total.Skip(startFrom).Take(blockSize).ToListAsync()).ProductListToPriceList(statistic);
                    startFrom += blockSize;
                    await priceRepository.SavePriceRangeAsync(prices);
                }
            }
            catch (Exception)
            {
                statistic.Status = -1;
                statisticRepository.SaveStatistic(statistic);
                Status = "Falled during Price snapshot";
            }
            finally
            {
                EmailNotifier.CreateMessage(Status, "Error");
            }
        }
    }
}
