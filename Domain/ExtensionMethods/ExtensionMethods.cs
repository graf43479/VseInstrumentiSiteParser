using System;
using System.Collections.Generic;
using Domain.Entity;
using Domain.Model;

namespace Domain.ExtensionMethods
{
    public static class ProductExtensions
    {
        public static Product CloneProduct(this Product productOrigin, Product product)
        {
            productOrigin.Code = product.Code;
            //productOrigin.CreationDate = product.CreationDate;
            productOrigin.UpdateDate = DateTime.Now;
            productOrigin.IsFavorite = product.IsFavorite;
            productOrigin.Name = product.Name;
            productOrigin.CurrentPrice = product.CurrentPrice;
            productOrigin.IsSale = product.IsSale;
            productOrigin.Rating = product.Rating;
            productOrigin.Responses = product.Responses;
            productOrigin.State = product.State;
            productOrigin.VendorID = product.VendorID;

            return productOrigin;
        }

        public static Price ProductToPrice(this Product product, Statistic statistic) 
        {
            return new Price
            {
                PriceValue = product.CurrentPrice,
                StatisticID = statistic.StatisticID,                
                ProductID = product.ProductID,
                Product = product,
                Statistic = statistic
            };
        }

        public static List<Price> ProductListToPriceList(this IEnumerable<Product> productList, Statistic statistic)
        {
            List<Price> priceList = new List<Price>();

            foreach (Product product in productList)
            {
                priceList.Add(product.ProductToPrice(statistic));
            }
            return priceList;
        }


        public static ProductSearchResultModel ProductToProductSearchModel(this Product product)
        {
            return new ProductSearchResultModel
            {
                Code = product.Code,
                VendorName = product.Vendor.Name,
                ProductName = product.Name,
                CreationDate = product.CreationDate.ToString("dd.MM.yyyy"),
                UpdateDate = product.UpdateDate.ToString("dd.MM.yyyy"),
                CurrentPrice = product.CurrentPrice,
                Rating = product.Rating,
                Responses = product.Responses,
                IsFavorite = (product.IsFavorite) ? "❤" : "-",
                IsSale = (product.IsSale) ? "Скидки" : "-",
                State = (product.State) ? "Есть" : "Нет"
            };
        }

        public static List<ProductSearchResultModel> ProductSearchListToModelList(this IEnumerable<Product> productList)
        {
            List<ProductSearchResultModel> prodList = new List<ProductSearchResultModel>();

            foreach (Product product in productList)
            {
                prodList.Add(product.ProductToProductSearchModel());
            }
            return prodList;
        }
        //ProductSearchResultModel

        //PriceDynamicModel


        //public static PriceDynamicModel GetPriceDynamicModel(this Product product)
        //{
        //    return new PriceDynamicModel
        //    {
        //        Code = product.Code,
                

        //        VendorName = product.Vendor.Name,
        //        ProductName = product.Name,
        //        CreationDate = product.CreationDate.ToString("dd.MM.yyyy"),
        //        UpdateDate = product.UpdateDate.ToString("dd.MM.yyyy"),
        //        CurrentPrice = product.CurrentPrice,
        //        Rating = product.Rating,
        //        Responses = product.Responses,
        //        IsFavorite = (product.IsFavorite) ? "❤" : "-",
        //        IsSale = (product.IsSale) ? "Скидки" : "-",
        //        State = (product.State) ? "Есть" : "Нет"
        //    };
        //}

        //public static List<ProductSearchResultModel> PriceListDynamicModel(this IEnumerable<Product> productList)
        //{
        //    List<ProductSearchResultModel> prodList = new List<ProductSearchResultModel>();

        //    foreach (Product product in productList)
        //    {
        //        prodList.Add(product.GetPriceDynamicModel());
        //    }
        //    return prodList;
        //}
    }
}
