using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Main.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string Coordinates { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public string Zip { get; set; }

    }
}
