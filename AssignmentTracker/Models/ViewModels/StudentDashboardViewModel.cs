namespace AssignmentTracker.Models.ViewModels
{
    public class StudentDashboardViewModel
    {
        public int TotalAssignments { get; set; }
        public int SubmittedCount { get; set; }
        public int PendingCount { get; set; }
        public double? AverageMarks { get; set; }
        public Assignment UpcomingAssignment { get; set; }
        public List<AssignmentStatusRow> Rows { get; set; } = new List<AssignmentStatusRow>();
    }

    public class AssignmentStatusRow
    {
        public Assignment Assignment { get; set; }
        public Submission Submission { get; set; }
    }
}
