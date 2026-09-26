# Assignment Submission Tracker

A comprehensive web-based assignment management system built with **ASP.NET Core MVC** (.NET 8) designed for educational institutions to streamline the assignment submission and evaluation process. Teachers create and grade assignments; students submit work and track marks. Authentication is session-based (no ASP.NET Core Identity), and the whole app is plain MVC — controllers talk to `ApplicationDbContext` directly (no repository pattern, no CQRS, no MediatR).

## Overview

Assignment Tracker is a role-based application that facilitates seamless communication between teachers and students regarding assignments. Teachers can create, manage, and evaluate assignments, while students can view, download, and submit their work with real-time tracking capabilities.

## Key Features

### For Teachers
- **Assignment Management**: Create, edit, and delete assignments with detailed descriptions and due dates.
- **File Uploads**: Upload assignment documents for distribution to students.
- **Submission Tracking**: View all student submissions for each assignment in a centralized dashboard.
- **Grading System**: Enter marks and provide feedback on student submissions.
- **Performance Analytics**: Track submission status (submitted, pending, late) at a glance.

### For Students
- **Assignment Dashboard**: View all active and past assignments with clear due dates.
- **File Download**: Download assignment documents for reference.
- **Submission Portal**: Upload solution files with secure file handling.
- **Submission History**: Track all submitted assignments and their evaluation status.
- **Grade Tracking**: View marks and feedback from teachers.

### General Features
- **Role-Based Authentication**: Secure login for teachers and students with encrypted passwords.
- **Session Management**: User sessions with automatic timeout for security.
- **Responsive Design**: Mobile-friendly interface for seamless access across devices.
- **Data Integrity**: Unique constraints ensure one submission per student per assignment.

## Tech Stack
- **Framework**: ASP.NET Core MVC, .NET 8, C#
- **Database**: SQL Server with Entity Framework Core (Code First) + SQL Server / LocalDB
- **Frontend**: Razor Views + Bootstrap 5 + Bootstrap Icons
- **Session Management**: Distributed Memory Cache
- **Language**: C#
- **ORM**: Code-First approach with migrations

## Project Folder Tree

```
AssignmentTracker/                        <- solution folder
├── AssignmentTracker.sln
├── README.md
├── .gitignore
├── Database/
│   └── CreateTables.sql                  <- manual SQL Server script (optional path)
└── AssignmentTracker/                    <- the MVC project
    ├── AssignmentTracker.csproj
    ├── Program.cs
    ├── appsettings.json
    ├── appsettings.Development.json
    ├── Controllers/
    │   ├── HomeController.cs
    │   ├── AccountController.cs
    │   ├── TeacherController.cs
    │   └── StudentController.cs
    ├── Filters/
    │   └── SessionAuthorizeAttribute.cs
    ├── Models/
    │   ├── Teacher.cs
    │   ├── Student.cs
    │   ├── Assignment.cs
    │   ├── Submission.cs
    │   └── ViewModels/
    │       ├── LoginViewModel.cs
    │       ├── TeacherDashboardViewModel.cs
    │       ├── StudentDashboardViewModel.cs
    │       └── AssignmentFormViewModel.cs
    ├── Data/
    │   ├── ApplicationDbContext.cs
    │   ├── DbInitializer.cs
    │   └── PasswordHelper.cs
    ├── Views/
    │   ├── _ViewImports.cshtml
    │   ├── _ViewStart.cshtml
    │   ├── Shared/
    │   │   ├── _Layout.cshtml
    │   │   └── Error.cshtml
    │   ├── Account/Login.cshtml
    │   ├── Teacher/
    │   │   ├── Dashboard.cshtml
    │   │   ├── Assignments.cshtml
    │   │   ├── CreateAssignment.cshtml
    │   │   ├── EditAssignment.cshtml
    │   │   ├── ViewAssignment.cshtml
    │   │   ├── ViewSubmissions.cshtml
    │   │   ├── EnterMarks.cshtml
    │   │   └── LateSubmissions.cshtml
    │   └── Student/
    │       ├── Dashboard.cshtml
    │       ├── Assignments.cshtml
    │       ├── Submit.cshtml
    │       └── Marks.cshtml
    └── wwwroot/
        ├── css/site.css
        ├── js/site.js
        └── uploads/
            ├── assignments/  (seeded demo files + real uploads land here)
            └── submissions/  (seeded demo file + real uploads land here)
```

## Database Diagram (ERD)

```
 Teachers                     Assignments                    Submissions                 Students
 ---------                    -----------                    -----------                 --------
 TeacherId (PK)  ───────┐     AssignmentId (PK)   ┐           SubmissionId (PK)           StudentId (PK) ───┐
 Name                   │     Title                │           AssignmentId (FK) ─────────┘                │
 Email (unique)         └───► TeacherId (FK)        │           StudentId (FK) ─────────────────────────────┘
 Password                     Description           │           SubmissionDate
                               Subject               │           FilePath
                               UploadDate            │           IsLate
                               DueDate               │           Marks
                               FilePath              │           Remarks
                               └── 1-to-many ────────┘
                               (one Teacher has many Assignments;
                                one Assignment has many Submissions;
                                one Student has many Submissions;
                                a Student can submit an Assignment only once —
                                unique index on (AssignmentId, StudentId))
```

## Demo Logins (seeded automatically on first run)

| Role    | Email                 | Password     |
|---------|------------------------|--------------|
| Teacher | teacher@college.edu    | Teacher@123  |
| Student | rahul@college.edu      | Student@123  |
| Student | priya@college.edu      | Student@123  |
| Student | aman@college.edu       | Student@123  |

## Run Instructions (Visual Studio 2022)

1. **Prerequisites**: Visual Studio 2022 (17.8+) with the "ASP.NET and web development" workload, .NET 8 SDK, and SQL Server LocalDB (installed by default with Visual Studio) or a full SQL Server instance.
2. Open `AssignmentTracker.sln` in Visual Studio 2022.
3. Restore NuGet packages (Visual Studio does this automatically on open, or right-click the solution → **Restore NuGet Packages**).
4. Check the connection string in `appsettings.json` — the default `(localdb)\mssqllocaldb` works out of the box with LocalDB. Change it if you want to point at a different SQL Server instance.
5. Open the **Package Manager Console** (Tools → NuGet Package Manager → Package Manager Console) with `AssignmentTracker` selected as the default project, and create the initial migration:
   ```
   Add-Migration InitialCreate
   Update-Database
   ```
   (Equivalently, from a terminal in the `AssignmentTracker` project folder:
   `dotnet ef migrations add InitialCreate` then `dotnet ef database update`.
   If `dotnet-ef` isn't installed: `dotnet tool install --global dotnet-ef`.)
6. Press **F5** (or **Ctrl+F5**) to run. `Program.cs` calls `context.Database.Migrate()` on startup, which applies the migration, and `DbInitializer.Seed()` inserts the demo teacher/students/assignments the first time the database is empty.
7. You'll land on the login page. Log in with any of the demo credentials above, or as a teacher to create your own assignments.

   Alternatively, you can skip EF migrations entirely and run `Database/CreateTables.sql` directly against SQL Server / LocalDB in SQL Server Management Studio or Azure Data Studio — the app will detect the existing schema and just seed data through `DbInitializer` on first run (skip step 5 in that case, but keep the connection string pointed at that database).

## Getting Started

### Prerequisites

- .NET 8 SDK or later
- SQL Server 2019 or later (Express edition supported)
- Visual Studio 2022 or Visual Studio Code
- Git

### Installation

1. **Clone the repository**
   ```
   git clone https://github.com/yourusername/AssignmentTracker.git
   cd AssignmentTracker
   ```

2. **Configure the database connection**

Update the connection string in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=AssignmentTrackerDB;Trusted_Connection=true;TrustServerCertificate=true;"
     }
   }

3. **Restore NuGet packages**
   ```
   dotnet restore
   ```

4. **Apply database migrations**
   ```
   dotnet ef database update
   ```

5. **Run the application**
   ```
   dotnet run
   ```

The application will be available at `https://localhost:5001` (or the configured port).

## Usage

### First-Time Login

The application comes with sample seed data:

**Teacher Account**
- Email: `teacher@college.edu`
- Password: `Teacher@123`

**Student Accounts**
- Email: `rahul@college.edu` | Password: `Student@123`
- Email: `priya@college.edu` | Password: `Student@123`
- Email: `aman@college.edu` | Password: `Student@123`

### Teacher Workflow

1. Log in with teacher credentials
2. Navigate to "Create Assignment"
3. Fill in assignment details and upload document
4. View submissions from students
5. Review and grade submissions with marks and feedback

### Student Workflow

1. Log in with student credentials
2. View all available and past assignments
3. Download assignment document for reference
4. Submit solution file before the due date
5. Check submitted status and grades from teachers

## Configuration

### Session Settings

Session timeout is configured to 60 minutes by default. Modify in `Program.cs`:

```csharp
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
```

### File Upload Paths

Default upload directories:
- Assignments: `wwwroot/uploads/assignments/`
- Submissions: `wwwroot/uploads/submissions/`

Ensure these directories have appropriate read/write permissions.

## Security Features

- **Password Hashing**: User passwords are hashed using secure algorithms.
- **Session Management**: Automatic session timeout after inactivity.
- **HttpOnly Cookies**: Prevents client-side script access to session cookies.
- **Unique Constraints**: Prevents duplicate submissions per assignment.
- **Role-Based Access**: Pages are restricted based on user role (Teacher/Student).

## Database Migrations

To create a new migration after model changes:

```
dotnet ef migrations add MigrationName
dotnet ef database update
```

## Troubleshooting

### Connection String Issues
- Verify SQL Server is running and accessible.
- Check firewall settings if connecting to a remote database.
- Ensure TrustServerCertificate is set to true for local development.

### File Upload Errors
- Verify upload directories exist in `wwwroot/uploads/`.
- Check file permissions for read/write access.
- Ensure sufficient disk space.

### Session Timeout
- Clear browser cookies if experiencing unexpected logouts.
- Increase timeout value in `Program.cs` if needed.

## Future Enhancements

- Email notifications for assignment deadlines and submissions.
- Assignment rubrics with detailed evaluation criteria.
- Bulk submission download functionality.
- Advanced filtering and search capabilities.
- Assignment plagiarism detection.
- Mobile application (iOS/Android).
- Real-time notifications using SignalR.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Contact & Support

For issues, questions, or suggestions, please create an issue in the repository or contact the development team.

## Contributors

- Development Team
- Educational Institution Partners

---

**Last Updated**: September 2026  
**Version**: 1.0.0
