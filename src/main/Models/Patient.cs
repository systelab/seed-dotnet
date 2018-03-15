using System;

namespace Main.Models
{
    public class Patient
    {
        public string Email { get; set; }
        public int Id { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public DateTime Dob { get; set; }
        public Address Address { get; set; }
    }
}