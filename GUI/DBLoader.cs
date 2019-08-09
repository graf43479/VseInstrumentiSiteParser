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
            IEnumerable<VendorProductCountModel>  pr = from p in productRepository.Products
                     group p by p.Vendor.Name into g
                     select new VendorProductCountModel{ VendorName = g.Key, Count = g.Count() };

            return pr;
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


        //internal List<PriceDynamicModel> GetPriceDynamicModels(string text, IEnumerable<TwoDaysPriceDiffereceModel> model)
        //{
        //  //  return model;

        //    //List<Product> products;
        //    //List<Price> prices;
        //    //int codeRes = 0;
        //    //if (int.TryParse(text, out codeRes))
        //    //{
        //    //    products = productRepository.Products.Where(x => x.Code == codeRes).AsNoTracking().ToList();
        //    //}
        //    //else
        //    //{
        //    //    products = productRepository.Products.Where(x => x.Name.ToLower().Contains(text.ToLower()) || x.Vendor.Name.ToLower().Contains(text.ToLower())).AsNoTracking().ToList();
        //    //}

        //   // return null; // products.PriceDynamicModel();
        //}

    }
}
