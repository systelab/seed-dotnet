namespace Main.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class UserViewModel
    {
        /// <summary>
        /// User's email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// User's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// User's lastname
        /// </summary>
        public string LastName { get; set; }
    }
}