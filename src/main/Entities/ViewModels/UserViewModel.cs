namespace main.Entities.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents a user
    /// </summary>
    public class UserViewModel
    {
        /// <summary>
        /// Gets or sets User's email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets User's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets User's lastname
        /// </summary>
        public string LastName { get; set; }
    }
}