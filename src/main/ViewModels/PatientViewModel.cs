namespace Main.ViewModels
{
    using Main.Models;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class PatientViewModel
    {
        /// <summary>
        /// Email address
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// Patient identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Patient last name (Surname)
        /// </summary>
        [Required]
        public string Surname { get; set; }

        /// <summary>
        /// Patient first name (Given name)
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Date
        /// </summary>
        public DateTime? Dob { get; set; }

        /// <summary>
        /// Address of the user
        /// </summary>
        public Address Address { get; set; }
    }
}