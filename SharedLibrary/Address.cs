using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public partial class Address
    {
        public List<Region> Region { get; set; }
        public List<Province> Province { get; set; }  
        public List<Municipality> Municipality { get; set; }
        public List<Barangay> Barangay { get; set; }
    }

    public partial class Region
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string RegionName { get; set; }        
    }

    public partial class Province
    {
        public string Code { get; set; }
        public string Name { get; set; }        
    }

    public partial class Municipality
    {
        public string Code { get; set; }
        public string Name { get; set; }           
        public bool IsCity { get; set; }
      
    }

    public partial class Barangay
    {
        public string Code { get; set; }
        public string Name { get; set; }                                                     
    }

    
    
    
    

}
