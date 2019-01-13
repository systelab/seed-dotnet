namespace main.Entities.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Representation of a patient
    /// </summary>
    public class AllergyViewModel
    {
  
        /// <summary>
        /// Gets or sets the allergy identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the allergy name
        /// </summary>
        [StringLength(255, ErrorMessage = "Name cannot be longer than 255 characters")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the allergy signs
        /// </summary>
        [StringLength(255, ErrorMessage = "Surname cannot be longer than 255 characters")]
        public string Signs { get; set; }

        /// <summary>
        /// Gets or sets the allergy symptoms
        /// </summary>
        [StringLength(255, ErrorMessage = "Medical number cannot be longer than 255 characters")]
        public string Symptoms { get; set; }
    }
}