//using System.ComponentModel.DataAnnotations;

namespace Domain.Entity
{
    public class Price
    {
        /// <summary>
        /// Id цены на товар
        /// </summary>        
        public int PriceID { get; set; }

        /// <summary>
        /// Внешний ключ цены на товар
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// Внешний ключ цены на статистику
        /// </summary>
        public int StatisticID { get; set; }

        /// <summary>
        /// Цена товара на момент даты CreationDate
        /// </summary>
        public int PriceValue { get; set; }

        /// <summary>
        /// Дата создания записи в базе
        /// </summary>
       // public DateTime CreationDate { get; set; }

        public virtual Product Product { get; set; }
        public virtual Statistic Statistic { get; set; }

        public override string ToString()
        {
            return PriceValue.ToString();
        }

    }
}
