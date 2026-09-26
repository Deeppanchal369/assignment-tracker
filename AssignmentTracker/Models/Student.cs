using System.ComponentModel.DataAnnotations;

namespace AssignmentTracker.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

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

        [Required(ErrorMessage = "Course is required")]
        [StringLength(100)]
        public string Course { get; set; }

        [Required(ErrorMessage = "Semester is required")]
        [StringLength(20)]
        public string Semester { get; set; }

        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
    }
}
