using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entity;

namespace Domain.DAL.Abstract
{
    public interface IStatisticRepository 
    {
        IQueryable<Statistic> Statistics { get; }
        void SaveStatistic(Statistic statistic);
        void DeleteStatistic(Statistic statistic);
        Statistic GetStatisticByID(int statisticId);
        Statistic GetStatisticByDate(DateTime date);
        Task SaveStatisticAsync(Statistic statistic);
        Task DeleteStatisticAsync(Statistic statistic);
        Task<Statistic> GetStatisticByIDAsync(int statisticId);
        Task<Statistic> GetStatisticByDateAsync(DateTime date);

        IEnumerable<DateTime> GetMonthlyStatistic();
    }
}
