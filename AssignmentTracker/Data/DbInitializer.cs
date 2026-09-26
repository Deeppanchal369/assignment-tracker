using AssignmentTracker.Models;

namespace AssignmentTracker.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Teachers.Any())
            {
                var teacher = new Teacher
                {
                    Name = "Dr. Anita Sharma",
                    Email = "teacher@college.edu",
                    Password = PasswordHelper.Hash("Teacher@123")
                };
                context.Teachers.Add(teacher);
                context.SaveChanges();

                var students = new List<Student>
                {
                    new Student { Name = "Rahul Verma", Email = "rahul@college.edu", Password = PasswordHelper.Hash("Student@123"), Course = "MCA", Semester = "III" },
                    new Student { Name = "Priya Singh", Email = "priya@college.edu", Password = PasswordHelper.Hash("Student@123"), Course = "MCA", Semester = "III" },
                    new Student { Name = "Aman Gupta", Email = "aman@college.edu", Password = PasswordHelper.Hash("Student@123"), Course = "MCA", Semester = "III" }
                };
                context.Students.AddRange(students);
                context.SaveChanges();

                var assignment1 = new Assignment
                {
                    Title = "ASP.NET Core MVC Basics",
                    Description = "Build a simple CRUD application demonstrating MVC architecture, model binding and validation.",
                    Subject = "Web Technologies",
                    UploadDate = DateTime.Today.AddDays(-10),
                    DueDate = DateTime.Today.AddDays(4),
                    FilePath = "/uploads/assignments/sample-assignment-1.pdf",
                    TeacherId = teacher.TeacherId
                };

                var assignment2 = new Assignment
                {
                    Title = "Entity Framework Core Migrations",
                    Description = "Demonstrate code-first migrations, relationships and seeding using EF Core.",
                    Subject = "Database Management Systems",
                    UploadDate = DateTime.Today.AddDays(-6),
                    DueDate = DateTime.Today.AddDays(-1),
                    FilePath = "/uploads/assignments/sample-assignment-2.pdf",
                    TeacherId = teacher.TeacherId
                };

                context.Assignments.AddRange(assignment1, assignment2);
                context.SaveChanges();

                var submission = new Submission
                {
                    AssignmentId = assignment2.AssignmentId,
                    StudentId = students[0].StudentId,
                    SubmissionDate = DateTime.Today.AddDays(-2),
                    FilePath = "/uploads/submissions/sample-submission-1.pdf",
                    IsLate = false,
                    Marks = 18,
                    Remarks = "Good understanding of relationships."
                };
                context.Submissions.Add(submission);
                context.SaveChanges();
            }
        }
    }
}
