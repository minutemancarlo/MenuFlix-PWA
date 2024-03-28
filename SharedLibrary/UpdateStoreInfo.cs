using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class UpdateStoreInfo
    {
        public int Id { get; set; }
        public string Logo { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone {  get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string CityTown { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public bool isLogoChanged { get; set; }
    }
}
