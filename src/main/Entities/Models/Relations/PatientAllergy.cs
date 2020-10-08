namespace Main.Entities.Models.Relations
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Main.Entities.Common;

    public class PatientAllergy : BaseEntity
    {
        public virtual Allergy Allergy { get; set; }

        [DataType(DataType.Date)]
        public DateTime AssertedDate { get; set; }

        [ForeignKey("Allergy")]
        public Guid IdAllergy { get; set; }

        [ForeignKey("Patient")]
        public Guid IdPatient { get; set; }

        [DataType(DataType.Date)]
        public DateTime LastOcurrence { get; set; }

        [Required]
        public string Note { get; set; }

        public virtual Patient Patient { get; set; }
    }
}