namespace Main.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class LoginViewModel
    {
        /// <summary>
        /// User's password
        /// </summary>
        [Required]
        public string password { get; set; }

        /// <summary>
        /// User's login identifier
        /// </summary>
        [Required]
        public string login { get; set; }
    }
}