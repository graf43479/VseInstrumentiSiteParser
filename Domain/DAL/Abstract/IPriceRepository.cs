using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entity;
using Domain.Model;

namespace Domain.DAL.Abstract
{
    public interface IPriceRepository
    {
        IQueryable<Price> Prices { get; }
        void SavePrice(Price price);
        void DeletePrice(Price price);
        Price GetPriceByID(int priceId);
        Task SavePriceAsync(Price price);
        Task DeletePriceAsync(Price price);
        Task<Price> GetPriceByIDAsync(int priceId);
        void DeletePriceRange(IEnumerable<Price> prices);
        void SavePriceRange(IEnumerable<Price> prices);
        Task DeletePriceRangeAsync(IEnumerable<Price> prices);
        Task SavePriceRangeAsync(IEnumerable<Price> prices);
        IEnumerable<Price> PriceListByProduct(Product product);
        Task<IEnumerable<Price>> PriceListByProductAsync(Product product);

        Task<IEnumerable<TwoDaysPriceDiffereceModel>> GetDiffernceAsync(DateTime dateStart, DateTime dateEnd, float percent, bool isChoosen);

        Task<IEnumerable<PriceDynamicModel>> GetDynamicAsync(bool isDaily, bool isFavorite, bool isNonConst, string searchText);

        IEnumerable<PriceDynamicModel> GetDynamic(bool isDaily, bool isFavorite, bool isNonConst, string searchText);
    }
}
