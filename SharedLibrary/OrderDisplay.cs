using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public partial class OrderDisplay
    {
        public List<OrderItem> OrderItems { get; set; }
        public List<UserAdditionalDetails>? UserAdditionalDetails { get; set; }
        public List<StatusEntry> Status {get;set;}
    }

    public partial class CashierDisplay
    {
        public OrderItem OrderItems { get; set; }
        public UserAdditionalDetails? UserAdditionalDetails { get; set; }
        public StatusEntry Status { get;set;}   
    }

    public partial class OrderJson
    {
     
        public string Item { get; set; }
        public string? UserAdditionalDetails { get; set; }
        public string Status { get; set; }
    }

    public partial class OrderItem : BaseModel
    {
        public bool IsReady { get; set; }
        public string OrderId { get; set; }
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemLogo { get; set; }
        public decimal Price { get; set; }
        public string Pcs { get; set; }
        public decimal SubTotal { get; set; }
        public string StoreName { get; set; }
        public string StoreLogo { get; set; }
        public string Category { get; set; }
        public decimal Total { get; set; }        
        
    }

    public partial class DeliveryInfo : UserAdditionalDetails
    {
        public string OrderId { get; set; }        
    }

    public partial class StatusEntry
    {
        public string OrderId { get; set; }
        public int Status { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        
    }
}
