using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class Store
    {        
        public required StoreApplications StoreInfo { get; set; }
        public required List<FoodItem> FoodItems { get; set; }

    }

    public partial class FoodItem : BaseModel
    {
        public int ItemId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Logo { get; set; }
        public decimal Price { get; set; }     
        public decimal Discount { get; set; }
        public string Email { get; set; }
        public string CategoryName { get; set; }
    }

    public partial class Category
    {
        public int CategoryId { get; set; }
        public string? Name { get; set; }
        public string? Description { get;set; }
    }

    public partial class Discounts
    {
        public int DiscountId { get; set; }
        public decimal DiscountPrice { get; set; }
        public int ItemId { get; set; }

    }
}
