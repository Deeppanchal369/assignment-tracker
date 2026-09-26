using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssignmentTracker.Models
{
    public class Submission
    {
        [Key]
        public int SubmissionId { get; set; }

        [Required]
        [ForeignKey(nameof(Assignment))]
        public int AssignmentId { get; set; }

        public Assignment Assignment { get; set; }

        [Required]
        [ForeignKey(nameof(Student))]
        public int StudentId { get; set; }

        public Student Student { get; set; }

        [Required]
        public DateTime SubmissionDate { get; set; }

        [Required]
        [StringLength(300)]
        public string FilePath { get; set; }

        public bool IsLate { get; set; }

        [Range(0, 100, ErrorMessage = "Marks must be between 0 and 100")]
        public int? Marks { get; set; }

        [StringLength(1000)]
        public string Remarks { get; set; }
    }
}
