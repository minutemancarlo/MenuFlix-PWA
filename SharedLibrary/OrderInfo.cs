using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class OrderInfo
    {
        public int Id { get; set; }
        public string OrderId { get; set; }
        public int ItemId { get; set; }
        public decimal Price { get; set; }
        public int Pcs { get; set; }
        public decimal SubTotal { get; set; }
        public string UserId { get; set; }
        public int StoreId { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
    }

}
