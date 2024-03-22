using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class FoodItemDiscount : BaseModel
    {
        public int DiscountId { get; set; }
        public string FoodItemId { get; set; }        
        public string DiscountName { get; set; }
        public decimal DiscountAmount { get; set; }
        public int isActive {  get; set; }
    }
}
