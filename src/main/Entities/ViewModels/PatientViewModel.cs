namespace main.Entities.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     Representation of a patient
    /// </summary>
    public class PatientViewModel
    {
        /// <summary>
        ///     Gets or sets the address of the user
        /// </summary>
        public AddressViewModel Address { get; set; }

        /// <summary>
        ///     Gets or sets the date of birth
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? Dob { get; set; }

        /// <summary>
        ///     Gets or sets email address
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        ///     Gets or sets the patient identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Gets or sets the patient first name (Given name)
        /// </summary>
        [Required]
        [StringLength(255, ErrorMessage = "Name cannot be longer than 255 characters")]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the patient last name (Surname)
        /// </summary>
        [Required]
        [StringLength(255, ErrorMessage = "Surname cannot be longer than 255 characters")]
        public string Surname { get; set; }

        /// <summary>
        ///     Gets or sets the patient medical number (Medical number)
        /// </summary>
        [StringLength(255, ErrorMessage = "Medical number cannot be longer than 255 characters")]
        public string MedicalNumber { get; set; }
    }
}