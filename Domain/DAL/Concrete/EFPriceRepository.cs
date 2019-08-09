using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Domain.DAL.Abstract;
using Domain.Entity;
using Domain.Model;
using System.Data;
using System.Data.SqlClient;

namespace Domain.DAL.Concrete
{
    public class EFPriceRepository : IPriceRepository
    {
        private ViDBContext context;

        public EFPriceRepository(ViDBContext context)
        {
            this.context = context;
        }

        public IQueryable<Price> Prices => context.Prices;

        public void DeletePrice(Price price)
        {
            try
            {
                context.Prices.Remove(price);
                context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeletePriceAsync(Price price)
        {
            try
            {
                context.Prices.Remove(price);
                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeletePriceRange(IEnumerable<Price> prices)
        {
            context.Prices.RemoveRange(prices);
            context.SaveChanges();
        }

        public async Task DeletePriceRangeAsync(IEnumerable<Price> prices)
        {
            context.Prices.RemoveRange(prices);
            await context.SaveChangesAsync();
        }

     

        public Price GetPriceByID(int priceId)
        {
            return Prices.FirstOrDefault(x=>x.PriceID==priceId);
        }

        public async Task<Price> GetPriceByIDAsync(int priceId)
        {
            return await Prices.FirstOrDefaultAsync(x => x.PriceID == priceId);
        }

        public IEnumerable<Price> PriceListByProduct(Product product)
        {
            return Prices.Where(x => x.ProductID == product.ProductID);
        }

        public async Task<IEnumerable<Price>> PriceListByProductAsync(Product product)
        {
            return await Prices.Where(x => x.ProductID == product.ProductID).AsNoTracking().ToListAsync();
        }

        public void SavePrice(Price price)
        {
            if (price.PriceID == 0)
            {
                context.Prices.Add(price);
            }
            else
            {
                context.Entry(price).State = EntityState.Modified;
            }
            context.SaveChanges();
        }

        public async Task SavePriceAsync(Price price)
        {
            if (price.PriceID == 0)
            {
                context.Prices.Add(price);
            }
            else
            {
                context.Entry(price).State = EntityState.Modified;
            }

            await context.SaveChangesAsync();
        }

        public void SavePriceRange(IEnumerable<Price> prices)
        {
            if (prices.Any(x => x.PriceID != 0))
            {
                foreach (Price item in prices)
                {
                    SavePrice(item);
                }
            }
            else
            {
                context.Prices.AddRange(prices);
                context.SaveChanges();
            }
        }
        //db.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s));
        public async Task SavePriceRangeAsync(IEnumerable<Price> prices)
        {            
                context.Prices.AddRange(prices);
                await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TwoDaysPriceDiffereceModel>> GetDiffernceAsync(DateTime dateStart, DateTime dateEnd, float percent, bool isChoosen)
        {           

            IEnumerable<TwoDaysPriceDiffereceModel> model = await context.Database.SqlQuery<TwoDaysPriceDiffereceModel>("GetTwoDaysChanges @param1, @param2, @paramPercent, @paramChoosen", 
                                                    new SqlParameter { ParameterName= "param1", SqlDbType=SqlDbType.DateTime2, Value=dateStart }, //("param1", dateStart),
                                                    new SqlParameter { ParameterName = "param2", SqlDbType = SqlDbType.DateTime2, Value = dateEnd }, //new SqlParameter("param2", dateEnd)
                                                    new SqlParameter { ParameterName = "paramPercent", SqlDbType = SqlDbType.Int, Value = percent },
                                                    new SqlParameter { ParameterName = "paramChoosen", SqlDbType = SqlDbType.Bit, Value = isChoosen }
                                                    ).ToListAsync();
            return model;
        }


        public async Task<IEnumerable<PriceDynamicModel>> GetDynamicAsync(bool isDaily, bool isFavorite, bool isNonConst, string searchText)
        {
            IEnumerable<PriceDynamicModel> model = await context.Database.SqlQuery<PriceDynamicModel>("GetAllStatisticPrices @param1, @param2, @param3",
                                                        new SqlParameter { ParameterName = "param1", SqlDbType = SqlDbType.Bit, Value = isDaily },
                                                        new SqlParameter { ParameterName = "param2", SqlDbType = SqlDbType.Bit, Value = isFavorite },
                                                        new SqlParameter { ParameterName = "param3", SqlDbType = SqlDbType.Bit, Value = isNonConst }
                                                    ).ToListAsync(); //ToList();
            if (String.IsNullOrEmpty(searchText))
                return model;

            int codeRes = 0;
            if (int.TryParse(searchText, out codeRes))
            {
                model = model.Where(x => x.Code == codeRes);
            }
            else
            {
                model = model.Where(x => x.Name.ToLower().Contains(searchText.ToLower()) || x.Vendor.ToLower().Contains(searchText.ToLower()));
            }
            return model;
        }

        public IEnumerable<PriceDynamicModel> GetDynamic(bool isDaily, bool isFavorite, bool isNonConst, string searchText)
        {
            IEnumerable<PriceDynamicModel> model = context.Database.SqlQuery<PriceDynamicModel>("GetAllStatisticPrices @param1, @param2, @param3",
                                                        new SqlParameter { ParameterName = "param1", SqlDbType = SqlDbType.Bit, Value = isDaily },
                                                        new SqlParameter { ParameterName = "param2", SqlDbType = SqlDbType.Bit, Value = isFavorite },
                                                        new SqlParameter { ParameterName = "param3", SqlDbType = SqlDbType.Bit, Value = isNonConst }
                                                    ).AsQueryable(); //ToList();
            int codeRes = 0;            
             if (int.TryParse(searchText, out codeRes))
            {
                model = model.Where(x => x.Code == codeRes).ToList();
            }
            else
            {
                model = model.Where(x => x.Name.ToLower().Contains(searchText.ToLower()) || x.Vendor.ToLower().Contains(searchText.ToLower())).ToList();
            }
            return model;
        }


    }
}
