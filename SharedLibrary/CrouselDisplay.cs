using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class CarouselDisplay
    {
        public string ItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; } = "icon-192.png";
        public decimal Price { get; set; }
        public decimal DiscountedAmount {  get; set; }
        public decimal DiscountedPrice {  get; set; }
        public string Category { get; set; }        
        public string Store { get; set; } 
        public string StoreLogo { get; set; } = "icon-192.png";
        public int Rating { get; set; } = 0;    
    }
}
