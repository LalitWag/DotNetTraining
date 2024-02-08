using System.ComponentModel.DataAnnotations;

namespace Project3._1BlazorServer.Data
{
    public class UserModel
    {
        [Required(ErrorMessage = "The Name field is required.")]
        [StringLength(50, ErrorMessage = "The Name field must be at most 50 characters long.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Email field is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }
    }
}
