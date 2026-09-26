# Assignment Submission Tracker

A comprehensive web-based assignment management system built with **ASP.NET Core 8.0 Razor Pages** designed for educational institutions to streamline the assignment submission and evaluation process. Teachers create and grade assignments; students submit work and track marks. Authentication is session-based (no ASP.NET Core Identity), with a lightweight architecture that keeps controllers simple and focused on business logic.

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

## Technology Stack

| Component | Technology |
|-----------|-----------|
| **Framework** | ASP.NET Core 8.0 (Razor Pages) |
| **Language** | C# |
| **Database** | SQL Server with Entity Framework Core 8.0.8 |
| **ORM Pattern** | Code-First approach with migrations |
| **Frontend** | Razor Pages + Bootstrap 5 + Bootstrap Icons |
| **Session Management** | Distributed Memory Cache |
| **Architecture Pattern** | Lightweight MVC (Controllers + Views/Razor Pages) |

## Project Structure

AssignmentTracker/                           <- Solution folder
├── AssignmentTracker.sln
├── README.md
├── .gitignore
└── AssignmentTracker/                       <- Main Razor Pages Project
    ├── AssignmentTracker.csproj
    ├── Program.cs                           <- Application configuration & startup
    ├── appsettings.json                     <- Base configuration (no secrets)
    ├── appsettings.Development.json         <- Development-specific settings
    │
    ├── Controllers/
    │   ├── AccountController.cs             <- Authentication (Login/Logout)
    │   ├── TeacherController.cs             <- Teacher assignment management
    │   ├── StudentController.cs             <- Student submission management
    │   └── HomeController.cs                <- Home page controller
    │
    ├── Filters/
    │   └── SessionAuthorizeAttribute.cs     <- Custom authorization filter
    │
    ├── Models/
    │   ├── Teacher.cs                       <- Teacher entity
    │   ├── Student.cs                       <- Student entity
    │   ├── Assignment.cs                    <- Assignment entity
    │   ├── Submission.cs                    <- Submission entity
    │   ├── PasswordHelper.cs                <- Password hashing utility
    │   └── ViewModels/
    │       ├── LoginViewModel.cs
    │       ├── TeacherDashboardViewModel.cs
    │       ├── StudentDashboardViewModel.cs
    │       └── AssignmentFormViewModel.cs
    │
    ├── Data/
    │   ├── ApplicationDbContext.cs          <- EF Core database context
    │   └── DbInitializer.cs                 <- Seed data configuration
    │
    ├── Views/
    │   ├── _ViewImports.cshtml              <- Shared namespaces & tag helpers
    │   ├── _ViewStart.cshtml                <- Default layout setup
    │   ├── Shared/
    │   │   ├── _Layout.cshtml               <- Master layout template
    │   │   ├── _NavBar.cshtml               <- Navigation bar partial
    │   │   └── Error.cshtml                 <- Error page
    │   ├── Account/
    │   │   └── Login.cshtml                 <- Login form
    │   ├── Teacher/
    │   │   ├── Dashboard.cshtml             <- Teacher dashboard
    │   │   ├── CreateAssignment.cshtml      <- Create new assignment form
    │   │   ├── EditAssignment.cshtml        <- Edit existing assignment
    │   │   ├── ViewAssignments.cshtml       <- List all assignments
    │   │   ├── ViewSubmissions.cshtml       <- View student submissions
    │   │   ├── EnterMarks.cshtml            <- Grade submission form
    │   │   └── LateSubmissions.cshtml       <- View late submissions
    │   └── Student/
    │       ├── Dashboard.cshtml             <- Student dashboard
    │       ├── Assignments.cshtml           <- View available assignments
    │       ├── Submit.cshtml                <- Submit assignment form
    │       └── Marks.cshtml                 <- View grades & feedback
    │
    └── wwwroot/
        ├── css/
        │   └── site.css                     <- Custom styles
        ├── js/
        │   └── site.js                      <- Custom scripts
        └── uploads/
            ├── assignments/                 <- Uploaded assignment documents
            └── submissions/                 <- Student submission files

## Database Schema (Entity-Relationship Diagram)

 Teachers                    Assignments                Submissions              Students
 --------                    -----------                -----------              --------
 TeacherId (PK) ─────┐       AssignmentId (PK)    ┐     SubmissionId (PK)       StudentId (PK)
 Name                 │       Title                │     AssignmentId (FK) ─┐    Name
 Email (unique)       └────►  TeacherId (FK)       │     StudentId (FK) ──┐ │    Email (unique)
 Password                     Description          │     SubmissionDate   │ │    Password
                              Subject               │     FilePath         │ │    Course
                              UploadDate            │     IsLate           │ │    Semester
                              DueDate               │     Marks            │ │
                              FilePath              │     Remarks          │ │
                              └── 1-to-many ────────┘     └────────────────┼─┘
                                                          Unique: (AssignmentId, StudentId)
                                                          (Prevents duplicate submissions)

### Entity Relationships
- **One Teacher** → **Many Assignments** (1:N)
- **One Assignment** → **Many Submissions** (1:N)
- **One Student** → **Many Submissions** (1:N)
- **Unique Constraint**: Each student can submit to an assignment only once

## Demo Login Credentials (Seeded on First Run)

| Role | Email | Password |
|------|-------|----------|
| Teacher | `teacher@college.edu` | `Teacher@123` |
| Student | `rahul@college.edu` | `Student@123` |
| Student | `priya@college.edu` | `Student@123` |
| Student | `aman@college.edu` | `Student@123` |

## Getting Started

### Prerequisites

- **.NET 8 SDK** or later ([download](https://dotnet.microsoft.com/download/dotnet/8.0))
- **Visual Studio 2022** (17.8+) with "ASP.NET and web development" workload
- **SQL Server** 2019+ (LocalDB, Express, or full edition)
- **Git** for version control

### Installation & Setup

#### Option 1: Visual Studio 2022 (Recommended)

1. **Clone the repository**
   git clone https://github.com/yourusername/AssignmentTracker.git
   cd AssignmentTracker

2. **Open the solution**
- Open `AssignmentTracker.sln` in Visual Studio 2022
- NuGet packages will restore automatically

3. **Configure database connection** (if needed)
- Open `appsettings.json`
- Default connection string uses LocalDB: `(localdb)\mssqllocaldb`
- Update if using a different SQL Server instance:
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=AssignmentTrackerDB;Trusted_Connection=true;TrustServerCertificate=true;"
     }
   }

4. **Create database & apply migrations**
- Open **Package Manager Console** (Tools → NuGet Package Manager → Package Manager Console)
- Ensure `AssignmentTracker` is selected as the default project
- Run:
   Add-Migration InitialCreate
   Update-Database

5. **Run the application**
- Press **F5** (Debug) or **Ctrl+F5** (Release)
- Application opens in your default browser at `https://localhost:5001`
- Login page appears automatically
- `DbInitializer.Seed()` runs automatically on first startup and populates demo data

#### Option 2: Command Line (.NET CLI)

# Clone the repository
git clone https://github.com/yourusername/AssignmentTracker.git
cd AssignmentTracker/AssignmentTracker

# Restore packages
dotnet restore

# Create & apply migrations
dotnet ef migrations add InitialCreate
dotnet ef database update

# Run the application
dotnet run
# Application runs on http://localhost:5000 or https://localhost:5001

#### Option 3: SQL Script (Manual Schema)

If you prefer to create the database manually:

1. Open **SQL Server Management Studio** or **Azure Data Studio**
2. Execute the SQL script (if provided in `Database/CreateTables.sql`)
3. Update connection string in `appsettings.json` to point to your database
4. Run the application — it will detect the schema and seed demo data automatically

## Usage Guide

### Teacher Workflow

1. **Log in** with teacher credentials (`teacher@college.edu` / `Teacher@123`)
2. **View Dashboard** — Overview of all assignments and submissions
3. **Create Assignment**
   - Click "Create Assignment"
   - Enter title, description, subject, due date
   - Upload assignment document
   - Click "Save"
4. **View Submissions** — See all student submissions for each assignment
5. **Grade Submissions**
   - Click "Enter Marks"
   - Enter marks and feedback/remarks
   - Click "Save"
6. **Track Late Submissions** — View assignments submitted after due date

### Student Workflow

1. **Log in** with student credentials (e.g., `rahul@college.edu` / `Student@123`)
2. **View Dashboard** — See all available and past assignments
3. **Download Assignment** — Click download icon to view assignment document
4. **Submit Assignment**
   - Click "Submit" for the assignment
   - Select the solution file
   - Click "Upload & Submit"
5. **Check Status** — View submission date and marks from teacher
6. **View Marks** — See grades and feedback in the "My Marks" section

## Configuration

### Session Settings

Session timeout is configured to **60 minutes** by default. To modify:

**File**: `Program.cs`
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);  // Change this value
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "AssignmentTracker.Session";
});

### File Upload Configuration

Default upload directories are created during migration:
- **Assignments**: `wwwroot/uploads/assignments/`
- **Submissions**: `wwwroot/uploads/submissions/`

Ensure these directories exist and have read/write permissions:
# Create directories if missing
mkdir wwwroot/uploads/assignments
mkdir wwwroot/uploads/submissions

### Logging Configuration

Modify log levels in `appsettings.json`:
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.EntityFrameworkCore": "Debug"
    }
  }
}

## Security Features

- **Password Hashing**: User passwords are hashed using secure algorithms before storage.
- **Session-Based Authentication**: Custom session management without ASP.NET Core Identity.
- **HttpOnly Cookies**: Prevents client-side JavaScript access to session cookies.
- **Unique Constraints**: Prevents duplicate submissions per assignment.
- **Role-Based Access**: Pages restricted based on user role (Teacher/Student).
- **Authorize Filters**: Custom `[SessionAuthorize]` attribute validates user sessions.
- **File Upload Validation**: Files are stored outside web root for security.

## Database Migrations

### Create a New Migration

After modifying entity models:

dotnet ef migrations add MigrationName
dotnet ef database update

### View Migration History

dotnet ef migrations list

### Revert Migration

dotnet ef database update PreviousMigrationName

### Reset Database (Development Only)

dotnet ef database drop
dotnet ef database update

## Project Build & Run

### Build the Project

dotnet build

### Run the Application

dotnet run

### Run in Release Mode

dotnet run --configuration Release

### Publish for Deployment

dotnet publish -c Release -o ./publish

## Troubleshooting

### Issue: Connection String Error
**Error**: `A network-related or instance-specific error occurred while establishing a connection`

**Solutions**:
- Verify SQL Server is running: `sqlcmd -S (localdb)\mssqllocaldb -U sa`
- Check connection string in `appsettings.json`
- Use SQL Server Management Studio to verify database exists
- Try connection with different credentials or server name

### Issue: Migration Failed
**Error**: `Failed to generate a SQL migration`

**Solutions**:
- Delete the `Migrations` folder and start fresh.
- Ensure models are valid and compile without errors.
- Run: `dotnet ef migrations add InitialCreate --force`.

### Issue: File Upload Not Working
**Error**: `The system cannot find the specified path`

**Solutions**:
- Verify directories exist: `wwwroot/uploads/assignments/` and `wwwroot/uploads/submissions/`.
- Check folder permissions (should allow read/write).
- Verify file size limits are appropriate in `Program.cs`.

### Issue: Session Timeout Unexpectedly
**Error**: User logged out after short inactivity

**Solutions**:
- Increase timeout in `Program.cs` (default: 60 minutes).
- Clear browser cookies and cache.
- Check if browser has "Clear cookies on exit" enabled.

### Issue: Demo Data Not Appearing
**Error**: Login fails with demo credentials

**Solutions**:
- Verify `DbInitializer.Seed()` was called on startup.
- Check database has `Teachers` and `Students` tables.
- Review Application logs for seed errors.
- Try deleting database and re-running migrations.

## Development

### Project Standards

- **Language Version**: C# Latest (11+)
- **Nullable Reference Types**: Disabled (`<Nullable>disable</Nullable>`)
- **Implicit Usings**: Enabled
- **Code Style**: Follows Microsoft C# conventions

### Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| `Microsoft.EntityFrameworkCore.SqlServer` | 8.0.8 | SQL Server database provider |
| `Microsoft.EntityFrameworkCore.Tools` | 8.0.8 | EF Core CLI commands |
| `Microsoft.EntityFrameworkCore.Design` | 8.0.8 | Design-time services |

### IDE Recommendations

- **Visual Studio 2022** (latest)
- **VS Code** with C# Dev Kit extension
- **Rider** by JetBrains

## Future Enhancements

- [ ] Email notifications for assignment deadlines and submissions
- [ ] Assignment rubrics with detailed evaluation criteria
- [ ] Bulk submission download functionality
- [ ] Advanced filtering and search capabilities
- [ ] Assignment plagiarism detection integration
- [ ] Real-time notifications using SignalR
- [ ] Mobile application (iOS/Android)
- [ ] Analytics dashboard with charts
- [ ] Automated backup system
- [ ] Two-factor authentication (2FA)

## License

This project is licensed under the **MIT License** — see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please follow these guidelines:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/YourFeature`)
3. Commit your changes (`git commit -m 'Add YourFeature'`)
4. Push to the branch (`git push origin feature/YourFeature`)
5. Open a Pull Request

## Support & Contact

For issues, questions, or suggestions:
- **Create an Issue** on GitHub
- **Contact**: development team
- **Email**: support@assignmenttracker.local

## Changelog

### Version 1.0.0 (Current)
- Initial release with core features
- Teacher assignment management
- Student submission system
- Session-based authentication
- Grading and feedback system

---

**Last Updated**: September 2026  
**Maintainers**: Development Team  
**Status**: Active Development
