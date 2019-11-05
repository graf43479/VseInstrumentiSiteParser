using Domain.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VseISiteParser.Interfaces
{
    /// <summary>
    /// сводный интерфейс для записи в БД
    /// </summary>
    public interface IDBLoader
    {
       Task<List<Vendor>> GetVendorsAsync();
      void LoadProductList(List<Product> products);
       IEnumerable<Statistic> GetStatistic();
       Task MakePriceSnapshotAsync();
    }
}
