using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Domain.DAL.Abstract;
using Domain.Entity;

namespace Domain.DAL.Concrete
{
    public class EFStatisticRepository : IStatisticRepository
    {
        private ViDBContext context;

        public EFStatisticRepository(ViDBContext context)
        {
            this.context = context;
        }

        public IQueryable<Statistic> Statistics => context.Statistics;

        public void DeleteStatistic(Statistic statistic)
        {
            try
            {
                context.Statistics.Remove(statistic);
                context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteStatisticAsync(Statistic statistic)
        {
            try
            {
                context.Statistics.Remove(statistic);
                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Statistic GetStatisticByDate(DateTime date)
        {
            return Statistics.FirstOrDefault(x => x.CreationDate.Date==date.Date);
        }

        public async Task<Statistic> GetStatisticByDateAsync(DateTime date)
        {
            return await Statistics.FirstOrDefaultAsync(x => x.CreationDate.Date == date.Date);
        }

        public Statistic GetStatisticByID(int statisticId)
        {
            return Statistics.FirstOrDefault(x => x.StatisticID== statisticId);
        }

        public async Task<Statistic> GetStatisticByIDAsync(int statisticId)
        {
            return await Statistics.FirstOrDefaultAsync(x => x.StatisticID == statisticId);
        }

        public void SaveStatistic(Statistic statistic)
        {
            if (statistic.StatisticID == 0)
            {
                context.Statistics.Add(statistic);
            }
            else
            {
                context.Entry(statistic).State = EntityState.Modified;
            }

            context.SaveChanges();
        }

        public async Task SaveStatisticAsync(Statistic statistic)
        {
            if (statistic.StatisticID == 0)
            {
                context.Statistics.Add(statistic);
            }
            else
            {
                context.Entry(statistic).State = EntityState.Modified;
            }

            await context.SaveChangesAsync();
        }

        public IEnumerable<DateTime> GetMonthlyStatistic()
        {
            IEnumerable<DateTime> model = context.Database.SqlQuery<DateTime>("GetMonthlyStatistic").AsQueryable(); //ToList();            
            return model;
        }
    }
}
