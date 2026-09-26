# Assignment Submission Tracker

An ASP.NET Core MVC (.NET 8) application for an MCA mini-project. Teachers create
and grade assignments; students submit work and track marks. Authentication is
session-based (no ASP.NET Core Identity), and the whole app is plain MVC —
controllers talk to `ApplicationDbContext` directly (no repository pattern, no
CQRS, no MediatR).

## Tech Stack
- ASP.NET Core MVC, .NET 8, C#
- Entity Framework Core (Code First) + SQL Server / LocalDB
- Razor Views + Bootstrap 5 + Bootstrap Icons
- Session-based authentication (`ISession`)

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

1. **Prerequisites**: Visual Studio 2022 (17.8+) with the "ASP.NET and web
   development" workload, .NET 8 SDK, and SQL Server LocalDB (installed by
   default with Visual Studio) or a full SQL Server instance.
2. Open `AssignmentTracker.sln` in Visual Studio 2022.
3. Restore NuGet packages (Visual Studio does this automatically on open, or
   right-click the solution → **Restore NuGet Packages**).
4. Check the connection string in `appsettings.json` — the default
   `(localdb)\mssqllocaldb` works out of the box with LocalDB. Change it if you
   want to point at a different SQL Server instance.
5. Open the **Package Manager Console** (Tools → NuGet Package Manager →
   Package Manager Console) with `AssignmentTracker` selected as the default
   project, and create the initial migration:
   ```
   Add-Migration InitialCreate
   Update-Database
   ```
   (Equivalently, from a terminal in the `AssignmentTracker` project folder:
   `dotnet ef migrations add InitialCreate` then `dotnet ef database update`.
   If `dotnet-ef` isn't installed: `dotnet tool install --global dotnet-ef`.)
6. Press **F5** (or **Ctrl+F5**) to run. `Program.cs` calls
   `context.Database.Migrate()` on startup, which applies the migration, and
   `DbInitializer.Seed()` inserts the demo teacher/students/assignments the
   first time the database is empty.
7. You'll land on the login page. Log in with any of the demo credentials
   above, or as a teacher to create your own assignments.

   Alternatively, you can skip EF migrations entirely and run
   `Database/CreateTables.sql` directly against SQL Server / LocalDB in SQL
   Server Management Studio or Azure Data Studio — the app will detect the
   existing schema and just seed data through `DbInitializer` on first run
   (skip step 5 in that case, but keep the connection string pointed at that
   database).

## Publish Instructions

**Publish to a folder / IIS:**
1. Right-click the `AssignmentTracker` project → **Publish**.
2. Choose **Folder** (or **IIS**, **Azure App Service**, etc. depending on your
   target) and follow the wizard.
3. Make sure the target server has the .NET 8 Hosting Bundle installed (for
   IIS) and that the `wwwroot/uploads` folder is writable by the application
   pool identity, since assignment/submission files are stored there.
4. Update the `DefaultConnection` string in the published `appsettings.json`
   to point at your production SQL Server, and run
   `Database/CreateTables.sql` against it (or let `Database.Migrate()` do it
   automatically on first launch, as it already does in `Program.cs`).

**Publish via CLI:**
```
dotnet publish AssignmentTracker/AssignmentTracker.csproj -c Release -o ./publish
```

## Notes on Business Rules
- **Late detection**: `Submission.IsLate` is computed server-side at the
  moment of submission — `SubmissionDate.Date > Assignment.DueDate.Date`.
- **Replace before deadline**: A student can only re-submit
  (`Student/Submit`) while `DateTime.Today <= Assignment.DueDate`; the
  controller blocks replacement after the deadline.
- **File validation**: enforced server-side in both `TeacherController` and
  `StudentController` (extension must be `.pdf`, `.doc`, or `.docx`; max size
  20 MB) and mirrored client-side in `site.js` for instant feedback.
- **Search & pagination**: `Teacher/Assignments` and `Student/Assignments`
  both accept `?search=` and `?page=` query parameters (5 rows per page).
