using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class MinimumPriceProductModel
    {
        public int ProductID { get; set; }

        public string VendorName { get; set; }
                
        public int Code { get; set; }

        /// <summary>
        /// полное имя товара
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// цена сейчас
        /// </summary>
        public int CurrentPrice { get; set; }

        /// <summary>
        /// Рейтинг товара 
        /// </summary>
        public float Rating { get; set; }

        /// <summary>
        /// Количество отзывов
        /// </summary>
        public short Responses { get; set; }

        /// <summary>
        /// Есть ли в продаже сейчас
        /// </summary>
        public bool State { get; set; }
        
    }
}
