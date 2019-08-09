
namespace VseInstrumenti.Core.Target
{
    public class VSSettings : IParserSettings
    {
        public string BaseURL { get; set; } = "https://www.vseinstrumenti.ru"; 
       
        public string Vendor { get; set; } = "{vendor}";

        public string Prefix { get; set; } = "page{currentId}";     

        public int StartPoint { get; set; }

        public int EndPoint { get; set; }

        public string[] SortList => new string[] { "?asc=DESC&orderby=price", "?orderby=sales_rating",  "?asc=ASC&orderby=price" };
        public VSSettings(int start, int end)
        {
            StartPoint = start;
            EndPoint = end;
        }
    }
}
