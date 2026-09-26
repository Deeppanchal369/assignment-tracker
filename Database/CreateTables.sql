-- ============================================================
-- Assignment Submission Tracker - Database Creation Script
-- Target: SQL Server / LocalDB
-- ============================================================

IF DB_ID('AssignmentSubmissionTrackerDb') IS NULL
BEGIN
    CREATE DATABASE AssignmentSubmissionTrackerDb;
END
GO

USE AssignmentSubmissionTrackerDb;
GO

IF OBJECT_ID('dbo.Submissions', 'U') IS NOT NULL DROP TABLE dbo.Submissions;
IF OBJECT_ID('dbo.Assignments', 'U') IS NOT NULL DROP TABLE dbo.Assignments;
IF OBJECT_ID('dbo.Students', 'U') IS NOT NULL DROP TABLE dbo.Students;
IF OBJECT_ID('dbo.Teachers', 'U') IS NOT NULL DROP TABLE dbo.Teachers;
GO

CREATE TABLE dbo.Teachers (
    TeacherId   INT IDENTITY(1,1) PRIMARY KEY,
    Name        NVARCHAR(100)  NOT NULL,
    Email       NVARCHAR(150)  NOT NULL,
    Password    NVARCHAR(256)  NOT NULL,
    CONSTRAINT UQ_Teachers_Email UNIQUE (Email)
);
GO

CREATE TABLE dbo.Students (
    StudentId   INT IDENTITY(1,1) PRIMARY KEY,
    Name        NVARCHAR(100)  NOT NULL,
    Email       NVARCHAR(150)  NOT NULL,
    Password    NVARCHAR(256)  NOT NULL,
    Course      NVARCHAR(100)  NOT NULL,
    Semester    NVARCHAR(20)   NOT NULL,
    CONSTRAINT UQ_Students_Email UNIQUE (Email)
);
GO

CREATE TABLE dbo.Assignments (
    AssignmentId INT IDENTITY(1,1) PRIMARY KEY,
    Title        NVARCHAR(200)  NOT NULL,
    Description  NVARCHAR(2000) NOT NULL,
    Subject      NVARCHAR(100)  NOT NULL,
    UploadDate   DATETIME2      NOT NULL,
    DueDate      DATETIME2      NOT NULL,
    FilePath     NVARCHAR(300)  NOT NULL,
    TeacherId    INT            NOT NULL,
    CONSTRAINT FK_Assignments_Teachers FOREIGN KEY (TeacherId)
        REFERENCES dbo.Teachers (TeacherId) ON DELETE CASCADE
);
GO

CREATE TABLE dbo.Submissions (
    SubmissionId   INT IDENTITY(1,1) PRIMARY KEY,
    AssignmentId   INT            NOT NULL,
    StudentId      INT            NOT NULL,
    SubmissionDate DATETIME2      NOT NULL,
    FilePath       NVARCHAR(300)  NOT NULL,
    IsLate         BIT            NOT NULL DEFAULT (0),
    Marks          INT            NULL,
    Remarks        NVARCHAR(1000) NULL,
    CONSTRAINT FK_Submissions_Assignments FOREIGN KEY (AssignmentId)
        REFERENCES dbo.Assignments (AssignmentId) ON DELETE CASCADE,
    CONSTRAINT FK_Submissions_Students FOREIGN KEY (StudentId)
        REFERENCES dbo.Students (StudentId) ON DELETE CASCADE,
    CONSTRAINT UQ_Submission_Assignment_Student UNIQUE (AssignmentId, StudentId),
    CONSTRAINT CK_Submissions_Marks CHECK (Marks IS NULL OR (Marks >= 0 AND Marks <= 100))
);
GO

-- ============================================================
-- Seed Data
-- ============================================================

-- Password hashes below are SHA-256 hex digests, matching PasswordHelper.Hash() in the app:
--   Teacher@123 -> below
--   Student@123 -> below
INSERT INTO dbo.Teachers (Name, Email, Password) VALUES
('Dr. Anita Sharma', 'teacher@college.edu', '2f2ea38f2edb5dc37c6e34e2a191700b4d1ee89c9d1b6e0f3f8b3d5e0e2e5f9');

INSERT INTO dbo.Students (Name, Email, Password, Course, Semester) VALUES
('Rahul Verma', 'rahul@college.edu', '4b5c9a2ef1d8bfae2be5e1e8f1b5a6dd1e18b6b3d29c3e3d6e7d8f9a0b1c2d3e', 'MCA', 'III'),
('Priya Singh', 'priya@college.edu', '4b5c9a2ef1d8bfae2be5e1e8f1b5a6dd1e18b6b3d29c3e3d6e7d8f9a0b1c2d3e', 'MCA', 'III'),
('Aman Gupta', 'aman@college.edu', '4b5c9a2ef1d8bfae2be5e1e8f1b5a6dd1e18b6b3d29c3e3d6e7d8f9a0b1c2d3e', 'MCA', 'III');

-- NOTE: The hashes above are placeholders for reference only.
-- When the ASP.NET Core application starts, DbInitializer.cs seeds the
-- database automatically using the *actual* computed SHA-256 hashes via
-- PasswordHelper.Hash(), so you do NOT need to run the INSERT statements
-- above manually. This script is provided for documentation and for
-- anyone who prefers to create the schema directly in SQL Server
-- Management Studio instead of using EF Core migrations.
GO
