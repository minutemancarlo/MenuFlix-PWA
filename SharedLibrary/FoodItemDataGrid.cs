using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class FoodItemDataGrid : BaseModel
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string Store {  get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DiscountedPrice { get; set; }
        public decimal Rating { get; set; }
        public int IsDisabled { get; set; }
    }
}
