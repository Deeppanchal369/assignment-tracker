using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssignmentTracker.Models
{
    public class Assignment
    {
        [Key]
        public int AssignmentId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Subject is required")]
        [StringLength(100)]
        public string Subject { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime UploadDate { get; set; }

        [Required(ErrorMessage = "Due date is required")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Required]
        [StringLength(300)]
        public string FilePath { get; set; }

        [Required]
        [ForeignKey(nameof(Teacher))]
        public int TeacherId { get; set; }

        public Teacher Teacher { get; set; }

        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();

        [NotMapped]
        public int TotalSubmissions => Submissions?.Count ?? 0;
    }
}
