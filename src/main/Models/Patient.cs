namespace Main.Models
{
    using System;

    public class Patient
    {
        public Address Address { get; set; }

        public DateTime Dob { get; set; }

        public string Email { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string MedicalNumber { get; set; }
    }
}