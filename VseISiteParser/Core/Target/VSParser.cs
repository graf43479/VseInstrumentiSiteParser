using System;
using System.Collections.Generic;
using Domain.Entity;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using VseInstrumenti.Interfaces;
using System.Threading;

namespace VseInstrumenti.Core.Target
{
    /// <summary>
    /// Класс для непосредственного парсинга свойств товаров
    /// </summary>
    public class VSParser : IParser<List<Product>>
    {
        /// <summary>
        /// Вытаскивает Product из html-тегов
        /// </summary>
        /// <param name="element">целевой html-элемент</param>
        /// <returns></returns>
        private Product GetProduct(IElement element)
        {
            var resp = element.QuerySelector("a.rating-count")?.TextContent.ToString().Replace(" ", String.Empty).Replace("\n", String.Empty);
            var isNAvail = element.QuerySelector(".not-available")?.TextContent;
            var curPrice = element.QuerySelector("div.price-actual span.amount")?.TextContent.Replace(" ", String.Empty);

            var rateingString = element.QuerySelector("div.rating-stars div.current")?.GetAttribute("style").Replace(".",","); //TextContent.Replace(" ", String.Empty);
                int rate_s = rateingString.IndexOf("(");
                int rate_e = rateingString.IndexOf(" *");
            float rating = float.Parse(rateingString.Substring(rate_s+1, rate_e - rate_s - 1));
            
            Product product = new Product
            {
                Code = Int32.Parse(element.QuerySelector("div.code .color-red").TextContent),
                Name = element.QuerySelector(".product-name a").GetAttribute("title"), ////.Replace("", String.Empty)  //надо тащить title
                IsSale = (element.QuerySelector(".nameplates-item") == null) ? false : true,
                Responses = (resp == null) ? (short)0 : short.Parse(resp),
                State = (isNAvail == null) ? true : false,
                CurrentPrice = (isNAvail == null) ? Int32.Parse(curPrice) : 0,
                UpdateDate = DateTime.Now,
                CreationDate = DateTime.Now,
                Rating = rating,
                Url = element.QuerySelector("a.link").GetAttribute("href")
            };
            return product;
        }

        /// <summary>
        /// Возвращает список товаров
        /// </summary>
        /// <param name="document">html-документ</param>
        /// <returns>список товаров или null</returns>
        public List<Product> Parse(IHtmlDocument document)
        {
            IDocument doc = document as Document;
            var list = new List<Product>();
            var items = document.QuerySelectorAll("#goodsListingBox .tile-box.product"); // product
            if (items == null)
                return null;

            foreach (IElement item in items)
            {
                try
                {
                    list.Add(GetProduct(item));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка перехвачена: " + ex.Message);
                }
            }            
            return list;
        }
    }
}
