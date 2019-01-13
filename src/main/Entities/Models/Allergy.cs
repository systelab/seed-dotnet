namespace main.Entities.Models
{
    using main.Entities.Common;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Allergy : BaseEntity
    {
        [Required]
        [StringLength(255, ErrorMessage = "Name cannot be longer than 255 characters")]
        public string Name { get; set; }
        [Required]
        [StringLength(255, ErrorMessage = "Sings cannot be longer than 255 characters")]
        public string Signs { get; set; }

        [StringLength(255, ErrorMessage = "Symptoms cannot be longer than 255 characters")]
        public string Symptoms { get; set; }
    }
}