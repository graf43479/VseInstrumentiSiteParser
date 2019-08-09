using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
//using AngleSharp.Dom.Html;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace VseInstrumenti.Core
{
   public interface IParser<T> where T:class
    {
        /// <summary>
        /// Возвращает T из html-документа
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        T Parse(IHtmlDocument document);        
    }
}
//ReadOnlyCollection<IWebElement>