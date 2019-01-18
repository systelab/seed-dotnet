using main.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace main.Entities.Models.Relations
{
    public class PatientAllergy : BaseEntity
    {
        [ForeignKey("Allergy")]
        public Guid IdAllergy { get; set; }

        [ForeignKey("Patient")]
        public Guid IdPatient { get; set; }

        public virtual Allergy Allergy { get; set; }

        public virtual Patient Patient { get; set; }

        [Required]
        public string Note { get; set; }

        [DataType(DataType.Date)]
        public DateTime LastOcurrence { get; set; }

        [DataType(DataType.Date)]
        public DateTime AssertedDate { get; set; }
    }
}
