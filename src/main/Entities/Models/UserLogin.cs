namespace Main.Entities.Models
{
    using System.ComponentModel.DataAnnotations;

    public class UserLogin
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }
    }
}