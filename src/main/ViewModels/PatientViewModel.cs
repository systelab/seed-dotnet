namespace seed_dotnet.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class PatientViewModel
    {
        [Required]
        public string Email { get; set; }

        public int id { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Name { get; set; }
    }
}