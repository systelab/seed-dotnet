using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace main.Entities.Models
{
    public class UserLogin
    {
        [Required]
        public string login { get; set; }
        [Required]
        public string password { get; set; }
    }
}