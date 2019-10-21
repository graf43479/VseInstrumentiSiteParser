using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
   public class TwoDaysPriceDiffereceModel
    {
        public string Vendor { get; set; }

        public int Code { get; set; }
        public string Name { get; set; }

        

        public int DateStart { get; set; }
        public int DateEnd { get; set; }
    }
}
