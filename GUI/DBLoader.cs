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
using Domain.Model;
using System.Data;

namespace GUI
{
   public class DBLoader
    {
        private IStatisticRepository statisticRepository; // = new EFStatisticRepository(db);
        private IVendorRepository vendorRepository; // = new EFVendorRepository(db);
        private IPriceRepository priceRepository; // = new EFPriceRepository(db);
        private IProductRepository productRepository; // = new EFProductRepository(db);

        public DBLoader(ViDBContext db)
        {            
            //using (ViDBContext db = new ViDBContext())
            statisticRepository = new EFStatisticRepository(db);
            vendorRepository = new EFVendorRepository(db);
            priceRepository = new EFPriceRepository(db);
            productRepository = new EFProductRepository(db);
        }


        public IEnumerable<VendorProductCountModel> GetVendorProductCount()
        {
            IEnumerable<VendorProductCountModel> pr = from p in productRepository.Products
                                                      group p by p.Vendor.Name into g
                                                      select new VendorProductCountModel { VendorName = g.Key, Count = g.Count() };

            return pr;
        }

        public IEnumerable<Vendor> GetVendors()
        {
            return vendorRepository.Vendors;
            
        }

        public async Task<IEnumerable<TwoDaysPriceDiffereceModel>> GetDiffernceAsync(DateTime dateStart, DateTime dateEnd, float percent, bool isChoosen)
        {
            return await priceRepository.GetDiffernceAsync(dateStart, dateEnd, percent, isChoosen);
        }

        internal List<ProductSearchResultModel> GetSearchResult(string text)
        {
            List<Product> products;
            int codeRes = 0;
            if (int.TryParse(text, out codeRes))
            {
                products = productRepository.Products.Where(x => x.Code == codeRes).AsNoTracking().ToList();
            }
            else
            {
                products = productRepository.Products.Where(x => x.Name.ToLower().Contains(text.ToLower()) || x.Vendor.Name.ToLower().Contains(text.ToLower())).AsNoTracking().ToList();
            }

            return products.ProductSearchListToModelList();
        }

        internal async Task SaveFavoriteAsync(ProductSearchResultModel productSearchResultModel)
        {
            //Product product = await productRepository.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Code == productSearchResultModel.Code);
            if (productSearchResultModel!= null)
            {
                //product.IsFavorite = !product.IsFavorite;
                await productRepository.ChangeFavoriteStatus(await productRepository.GetProductByCodeAsync(productSearchResultModel.Code));
            }
        }

        public async Task<IEnumerable<PriceDynamicModel>> GetDynamicAsync(bool isDaily, bool isFavorite, bool isNonConst, string searchText)
        {            
            return await priceRepository.GetDynamicAsync(isDaily, isFavorite, isNonConst, searchText);
        }

        public IEnumerable<PriceDynamicModel> GetDynamic(bool isDaily, bool isFavorite, bool isNonConst, string searchText)
        {            
            return priceRepository.GetDynamic(isDaily, isFavorite, isNonConst, searchText);
        }

        public IEnumerable<Statistic> GetTopStatistic(bool isDaily, int count)
        {
            if (isDaily)
            {
                return statisticRepository.Statistics.OrderByDescending(x => x.CreationDate).Take(count).OrderBy(x=>x.CreationDate).AsNoTracking();
            }

            //statisticRepository.Statistics.Intersect(x=>x.)
            List<Statistic> statisticMonthly = new List<Statistic>();
            List<DateTime> statisticDatesMonthly = statisticRepository.GetMonthlyStatistic().ToList() ;
            var p = statisticRepository.Statistics.Where(x => x.CreationDate == statisticDatesMonthly.FirstOrDefault(y=>y == x.CreationDate));
            return p.OrderByDescending(x => x.CreationDate).Take(count).OrderBy(x => x.CreationDate).AsNoTracking();
            //return statisticRepository.GetMonthlyStatistic() //Statistics.OrderBy(x => x.CreationDate).Take(2).AsNoTracking();

        }


        public async Task<List<DateTime>> GetDatesAsync(bool isDaily)
        {
            if (isDaily)
            {
                return await statisticRepository.Statistics.OrderByDescending(x => x.CreationDate).Take(7).OrderBy(x=>x.CreationDate).Select(x => x.CreationDate).ToListAsync(); //priceRepository.GetDynamic(isDaily, isFavorite, isNonConst, searchText);
            }
            else
            {
                DateTime date = DateTime.Now;
                List<DateTime> dates = new List<DateTime>();
                for (int i = 0; i < 7; i++)
                {
                    dates.Add(DateTime.Now.AddMonths(-i));
                }
                return dates.OrderBy(x=>x).ToList();
            }
        }
                       

        internal async Task<string> GetUrlAsync(int code)
        {
            return "https://www.vseinstrumenti.ru" + (await productRepository.Products.FirstOrDefaultAsync(x => x.Code == code)).Url;
        }

        internal async Task<IEnumerable<VendorInfo>> GetVendorsAsync()
        {

            //IEnumerable<VendorInfo> model = await (from v in vendorRepository.Vendors
            //            join p in productRepository.Products on v.VendorID equals p.VendorID
            //            group p by new { p.Vendor.Name } into g
            //            select new VendorInfo()
            //            {
            //                VendorName = g.Key.Name,
            //                ProductCount = g.Count(),
            //                FavoriteProductCount = g.Where(x => x.IsFavorite).Count()
            //            }).ToListAsync();

            //TODO: отображать вендора, даже если нет товаров
          
            IEnumerable<VendorInfo> model = await (from v in vendorRepository.Vendors
                                                   join p in productRepository.Products.Include(x => x.Vendor) on v equals p.Vendor
                                                   into GroupJoin
                                                   from r in GroupJoin.DefaultIfEmpty()

                                                   group r by new { r.Vendor.Name } into g
                                                   select new VendorInfo()
                                                   {
                                                       VendorName = (g.Key.Name == null ? "XZ" : g.Key.Name),
                                                       ProductCount = g.Count(),
                                                       FavoriteProductCount = g.Where(x => x.IsFavorite).Count()
                                                   }).ToListAsync();
            return model;
        }

        public int GetExtremumValue(bool isMinimum)
        {
            int val = 0;
            if (isMinimum)
            {
                val = priceRepository.Prices.Where(x => x.PriceValue != 0).Min(x => x.PriceValue);
            }
            else
            {
                val = priceRepository.Prices.Where(x => x.PriceValue != 0).Max(x => x.PriceValue);
            }
            return val;
        }

        public async Task<IEnumerable<MinimumPriceProductModel>> GetProductCurrentMinValueAsync(bool dailyMin, string vendorName, string searchString, int rangeStart, int rangeEnd)
        {
            //Актуальная дата в статистике (сегодня)
            DateTime actualDate = statisticRepository.Statistics.Max(x => x.CreationDate);
            //Все цены за актуальную дату
            var curPrices = from pr in priceRepository.Prices
                      join s in statisticRepository.Statistics
                      on pr.StatisticID equals s.StatisticID
                      where s.CreationDate == actualDate && pr.PriceValue != 0
                      select new
                      {
                          ProductId = pr.ProductID,
                          Price = pr.PriceValue
                      };

            //Исторически минимальные значения цены в разбивке по товару
            var minPrices = from pr in priceRepository.Prices
                            where pr.PriceValue != 0
                            group pr by pr.ProductID into g
                            select new
                            {
                                ProductId = g.Key,
                                Price = g.Min(x=>x.PriceValue)
                            };
         
            var result = curPrices.Intersect(minPrices);            
            //если мы хотим узнать товары, не просто с минимальной ценой, а с минимальной за всю историю наблюдений, но изменившиеся сегодня
            if (dailyMin)
            {
                DateTime yesterdayDate = statisticRepository.Statistics.Where(x => x.CreationDate != actualDate).Max(x => x.CreationDate);
                var yesterdayPrices = from pr in priceRepository.Prices
                                      join s in statisticRepository.Statistics
                                      on pr.StatisticID equals s.StatisticID
                                      where s.CreationDate == yesterdayDate && pr.PriceValue != 0
                                      select new
                                      {
                                          ProductId = pr.ProductID,
                                          Price = pr.PriceValue
                                      };
                //исключаем из сегодняшних вчерашние
                var yresult = yesterdayPrices.Intersect(minPrices);

                result = result.Except(yresult);
            }
            
            var products = (from p in productRepository.Products
                            join r in result on p.ProductID equals r.ProductId
                            select p);

            if (vendorName != null)
            {
                products = from p in products
                           join v in vendorRepository.Vendors
                           on p.VendorID equals v.VendorID
                           where v.Name == vendorName
                           select p;
            }

            if (searchString != null)
            {
                products = products.Where(x => x.Name.ToLower().Contains(searchString.ToLower()));                           
            }
            
            return await (products.Where(x=>x.CurrentPrice>=rangeStart && x.CurrentPrice<=rangeEnd).Select(x=>new MinimumPriceProductModel() 
            {
                Code = x.Code,
                CurrentPrice = x.CurrentPrice,
                Name = x.Name,
                ProductID=x.ProductID,
                Rating = x.Rating,
                Responses = x.Responses,
                State=x.State,
                VendorName = x.Vendor.Name
            })).ToListAsync();
        }

        internal async Task<bool> CreateOrUpdateVendorAsync(string name, string subUrl)
        {
            if (!vendorRepository.Vendors.Where(x => x.Name == name || x.SubUrl == subUrl).Any())
            {
                try
                {
                    Vendor vendor = new Vendor() { Name = name, SubUrl = subUrl, CreationDate = DateTime.Now, UpdateDate = DateTime.Now };
                    await vendorRepository.SaveVendorAsync(vendor);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }                
            }
            else
            {
                return false;
            }
        }

        internal async Task<bool> DeleteVendorAsync(string vendorName)
        {
            Vendor vendor = await vendorRepository.Vendors.FirstOrDefaultAsync(x => x.Name == vendorName);
            if (vendor!=null)
            {
                try
                {
                    await vendorRepository.DeleteVendorAsync(vendor);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        internal async Task<List<int>> GetPriceHistoryAsync(int productID)
        {
            List<int> prices = await priceRepository.Prices.Where(x => x.ProductID == productID).OrderBy(x=>x.PriceValue).Select(x => x.PriceValue).Distinct().ToListAsync();
            return prices;
        }
    }

}
