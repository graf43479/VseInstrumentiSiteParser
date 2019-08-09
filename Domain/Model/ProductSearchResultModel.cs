using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class ProductSearchResultModel
    {


        public string VendorName { get; set; }
        public int Code { get; set; }

        public string ProductName { get; set; }

        public int CurrentPrice { get; set; }

        public float Rating { get; set; }

        public short Responses { get; set; }

        public string State { get; set; }


        public string IsSale { get; set; }

        public string IsFavorite { get; set; }

        public string CreationDate { get; set; }

        public string UpdateDate { get; set; }
        
    }
}
