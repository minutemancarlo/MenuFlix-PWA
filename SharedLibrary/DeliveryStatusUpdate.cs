using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class DeliveryStatusUpdate
    {
        public string OrderId { get; set; }
        public string EmailAddress { get; set; }
        public int Status { get; set; }
    }
}
