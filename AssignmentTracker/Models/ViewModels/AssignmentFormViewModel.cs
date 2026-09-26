using System.ComponentModel.DataAnnotations;

namespace AssignmentTracker.Models.ViewModels
{
    public class AssignmentFormViewModel
    {
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

        [Required(ErrorMessage = "Due date is required")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; } = DateTime.Today.AddDays(7);

        public IFormFile File { get; set; }

        public string ExistingFilePath { get; set; }
    }
}
