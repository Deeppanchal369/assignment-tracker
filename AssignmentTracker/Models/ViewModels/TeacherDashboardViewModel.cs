namespace AssignmentTracker.Models.ViewModels
{
    public class TeacherDashboardViewModel
    {
        public int TotalStudents { get; set; }
        public int TotalAssignments { get; set; }
        public int PendingSubmissions { get; set; }
        public int LateSubmissions { get; set; }
        public List<Assignment> RecentAssignments { get; set; } = new List<Assignment>();
    }
}
