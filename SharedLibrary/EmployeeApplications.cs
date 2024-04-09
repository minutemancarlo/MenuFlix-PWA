using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class EmployeeApplications
    {
        public int ApplicationId { get; set; }
        public int Position { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set;}
        public string CityTown { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public string UserId { get; set; }
        public int Status { get; set; }
    }
}
