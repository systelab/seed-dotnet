namespace Main.Entities.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Main.Entities.Common;

    public class Patient : BaseEntity
    {
        public Address Address { get; set; }

        [DataType(DataType.Date)]
        public DateTime Dob { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [StringLength(255, ErrorMessage = "Medical number cannot be longer than 255 characters")]
        public string MedicalNumber { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "Name cannot be longer than 255 characters")]
        public string Name { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "Surname cannot be longer than 255 characters")]
        public string Surname { get; set; }
    }
}