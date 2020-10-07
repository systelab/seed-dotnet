namespace main.Entities.ViewModels.Relations
{
    using System;

    using main.Entities.Models;

    public class PatientAllergyViewModel
    {
        public Allergy Allergy { get; set; }

        public DateTime AssertedDate { get; set; }

        public Guid Id { get; set; }

        public Guid IdAllergy { get; set; }

        public Guid IdPatient { get; set; }

        public DateTime LastOcurrence { get; set; }

        public string Note { get; set; }
    }
}