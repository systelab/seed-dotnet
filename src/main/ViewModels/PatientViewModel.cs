namespace Main.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Representation of a patient
    /// </summary>
    public class PatientViewModel
    {
        /// <summary>
        /// Gets or sets the address of the user
        /// </summary>
        public AddressViewModel Address { get; set; }

        /// <summary>
        /// Gets or sets the date of birth
        /// </summary>
        public DateTime? Dob { get; set; }

        /// <summary>
        /// Gets or sets email address
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the patient identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the patient first name (Given name)
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the patient last name (Surname)
        /// </summary>
        [Required]
        public string Surname { get; set; }
    }
}