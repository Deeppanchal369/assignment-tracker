using System.ComponentModel.DataAnnotations;

namespace AssignmentTracker.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please select a role")]
        public string Role { get; set; } // "Teacher" or "Student"
    }
}
