using System.ComponentModel.DataAnnotations;

namespace AssignmentTracker.Models
{
    public class Teacher
    {
        [Key]
        public int TeacherId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        [StringLength(150)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(256)]
        public string Password { get; set; }

        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    }
}
