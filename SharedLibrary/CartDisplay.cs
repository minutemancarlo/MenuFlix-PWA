using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SharedLibrary
{
    public class CartDisplay
    {
        public int CartId { get; set; }
        public int Pcs { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountedPrice { get; set; }
        public string Category { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DiscountedTotal { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
    }
}
