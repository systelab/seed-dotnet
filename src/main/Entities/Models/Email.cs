using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace main.Entities.Common
{
    public class Email
    {
        public string subject { get; set; }
        public string body { get; set; }
        public string emailTo { get; set; }
    }
}
