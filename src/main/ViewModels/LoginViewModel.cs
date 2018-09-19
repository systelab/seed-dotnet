namespace Main.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Representation of a login
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Gets or sets User's password
        /// </summary>
        [Required]
        public string password { get; set; }

        /// <summary>
        /// Gets or sets User's login identifier
        /// </summary>
        [Required]
        public string login { get; set; }
    }
}