namespace Main.Models
{
    using main.Models;
    using System;

    public class Patient : BaseEntity
    {
        public Address Address { get; set; }

        public DateTime Dob { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string MedicalNumber { get; set; }
    }
}