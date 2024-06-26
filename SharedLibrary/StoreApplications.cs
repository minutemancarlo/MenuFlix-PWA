﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class StoreApplications
    {                
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string TIN { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string CityTown { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string OwnerId { get; set; }
        public string Logo { get; set; }
        public string GcashQr { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }        

    }
}
