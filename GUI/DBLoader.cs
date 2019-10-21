﻿using System;
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
            
            IEnumerable<VendorInfo> model = await (from v in vendorRepository.Vendors
                        join p in productRepository.Products on v.VendorID equals p.VendorID
                        group p by new { p.Vendor.Name } into g
                        select new VendorInfo()
                        {
                            VendorName = g.Key.Name,
                            ProductCount = g.Count(),
                            FavoriteProductCount = g.Where(x => x.IsFavorite).Count()
                        }).ToListAsync();

            //var model = productRepository.Products
            //        .GroupJoin(
            //            vendorRepository.Vendors,
            //            e=>e.VendorID,
            //            o => o.VendorID,
            //            (e,os) => new VendorInfo
            //            {
            //                VendorName = e.Vendor.Name,
            //                ProductCount = os.Count(o=>o.pr)
            //            }


                        //    )

            
            
            return model;
        }

        

       
    }
}
