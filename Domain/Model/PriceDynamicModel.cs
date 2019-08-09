using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain.Model
{
    public class PriceDynamicModel
    {
        public string Vendor { get; set; }

        public string Name { get; set; }
        
        public int Code { get; set; }

        public bool IsFavorite{ get; set; }


        //public List<int> Date { get; set; }

        public int? Date0 { get; set; }

        public int? Date1 { get; set; }

        public int? Date2 { get; set; }

        public int? Date3 { get; set; }

        public int? Date4 { get; set; }

        public int? Date5 { get; set; }

        public int? Date6 { get; set; }

    }
}
