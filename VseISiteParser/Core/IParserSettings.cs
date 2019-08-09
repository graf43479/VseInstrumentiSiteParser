using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VseInstrumenti.Core
{
    /// <summary>
    /// Интерфейс по работе с URL
    /// </summary>
    public interface IParserSettings 
    {
        /// <summary>
        /// Основной url сайта
        /// </summary>
        string BaseURL { get; }

        /// <summary>
        /// Подкаталог
        /// </summary>
        string Vendor { get;  }
        
        /// <summary>
        /// Дополнительный параметр запроса
        /// </summary>
        string Prefix { get;  }            

        /// <summary>
        /// Номер стартовой страницы поиска
        /// </summary>
        int StartPoint { get; set; }

        /// <summary>
        /// Номер конечная страницы поиска
        /// </summary>
        int EndPoint { get; set; }

        string[] SortList { get;  }
    }
}
