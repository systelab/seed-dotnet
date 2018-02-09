namespace seed_dotnet.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class LoginViewModel
    {
        [Required]
        public string Password { get; set; }

        [Required]
        public string UserName { get; set; }
    }
}