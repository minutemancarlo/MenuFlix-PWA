using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class EmployeeStatusType
    {
        public int ApplicationId { get; set; }
        public int? Status { get; set; }
        public string? UserId { get; set; }
        public int? Position { get; set; }
    }
}
