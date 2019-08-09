using System;
using System.Collections.Generic;

namespace Domain.Entity
{
    public class Vendor
    {
        /// <summary>
        /// Уникальный код вендора в базе
        /// </summary>        
        public int VendorID { get; set; }

        /// <summary>
        /// Имя вендора (возможено на русском)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Url-ключ для поиска (англ)
        /// </summary>
        public string SubUrl { get; set; }

        /// <summary>
        /// Дата создания записи в базе
        /// </summary>
        public DateTime CreationDate { get; set; }


        /// <summary>
        /// Дата обновленич записи в базе
        /// </summary>
        public DateTime UpdateDate { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public Vendor()
        {
            Products = new List<Product>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
