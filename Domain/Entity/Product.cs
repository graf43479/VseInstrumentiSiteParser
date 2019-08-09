using System;
using System.Collections.Generic;
using Domain.Model;

namespace Domain.Entity
{
   public class Product
    {
        /// <summary>
        /// Id товара в базе
        /// </summary>        
        public int ProductID { get; set; }

        /// <summary>
        /// Внешний ключ на вендора
        /// </summary>
        public int VendorID { get; set; }

        /// <summary>
        /// Уникальный код товара по маркировке ВИ
        /// </summary>
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
        public bool State  { get; set; }      


        /// <summary>
        /// Участвует ли товар в распродаже сейчас
        /// </summary>
        public bool IsSale { get; set; }

        /// <summary>
        /// Флаг избранного товара
        /// </summary>
        public bool IsFavorite { get; set; }

        /// <summary>
        /// Дата создания записи в базе
        /// </summary>
        public DateTime CreationDate { get; set; }
        
        /// <summary>
        /// Дата обновления записи в базе
        /// </summary>
        public DateTime UpdateDate { get; set; }

        public virtual Vendor Vendor { get; set; }

        public virtual ICollection<Price> Prices { get; set; }

        public Product()
        {
            Prices = new List<Price>();
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            Product product = obj as Product;
            if (product != null)
            {
                return (this.Name == product.Name)
                && (this.Rating == product.Rating)
                && (this.CurrentPrice == product.CurrentPrice)
                && (this.IsSale == product.IsSale)
                && (this.Responses == product.Responses)
                && (this.State == product.State);
              //  && (this.IsFavorite == product.IsFavorite);
            }
            return false;
            
        }

      
    }
}
