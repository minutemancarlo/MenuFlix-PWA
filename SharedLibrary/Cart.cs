using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class Cart
    {
        public int CartId { get; set; }
        public int ItemId { get; set; }
        public int Actions { get; set; }
        public int Pcs { get; set; }
        public string UserId { get; set; }
        public int IsProcessed { get; set; }

    }
}
