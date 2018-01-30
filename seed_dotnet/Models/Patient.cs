using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace seed_dotnet.Models
{
    public class Patient
    {
        public int id { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
