using System;
using System.Collections.Generic;

namespace Domain.Entity
{
    public class Statistic
    {
        /// <summary>
        /// ID сессии
        /// </summary>        
        public int StatisticID { get; set; }        

        /// <summary>
        /// Статус сессии //удачно или нет
        /// </summary>
        public short Status { get; set; }

        /// <summary>
        /// Дата сессии
        /// </summary>
        public DateTime CreationDate { get; set; }

        public virtual ICollection<Price> Prices { get; set; }

        public Statistic()
        {
            Prices = new List<Price>();
        }
    }
}
