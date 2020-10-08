namespace Main.Entities.Models
{
    using System.ComponentModel.DataAnnotations;

    using Main.Entities.Common;

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