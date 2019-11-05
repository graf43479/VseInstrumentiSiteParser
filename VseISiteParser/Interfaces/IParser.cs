using AngleSharp.Html.Dom;

namespace VseInstrumenti.Interfaces
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