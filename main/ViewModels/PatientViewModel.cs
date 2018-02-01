using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace seed_dotnet.ViewModels
{
    public class PatientViewModel
    {

        public int id { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }

    }
}
