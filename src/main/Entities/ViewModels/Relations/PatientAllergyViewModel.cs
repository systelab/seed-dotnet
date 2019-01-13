using main.Entities.Models;
using System;

namespace main.Entities.ViewModels.Relations
{
  
    public class PatientAllergyViewModel
    {

        public Guid IdAllergy { get; set; }

        public Guid IdPatient { get; set; }

        public Guid Id { get; set; }

        public DateTime LastOcurrence { get; set; }

        public DateTime AssertedDate { get; set; }

        public Allergy Allergy { get; set; }

        public string Note { get; set; }
    }
}