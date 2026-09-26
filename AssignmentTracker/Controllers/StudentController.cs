using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssignmentTracker.Data;
using AssignmentTracker.Filters;
using AssignmentTracker.Models;
using AssignmentTracker.Models.ViewModels;

namespace AssignmentTracker.Controllers
{
    [SessionAuthorize("Student")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private const long MaxFileSize = 20 * 1024 * 1024; // 20 MB
        private static readonly string[] AllowedExtensions = { ".pdf", ".doc", ".docx" };

        public StudentController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        private int StudentId => HttpContext.Session.GetInt32("UserId") ?? 0;

        // GET: /Student/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var studentId = StudentId;

            var assignments = await _context.Assignments
                .Include(a => a.Submissions.Where(s => s.StudentId == studentId))
                .OrderBy(a => a.DueDate)
                .ToListAsync();

            var rows = assignments.Select(a => new AssignmentStatusRow
            {
                Assignment = a,
                Submission = a.Submissions.FirstOrDefault(s => s.StudentId == studentId)
            }).ToList();

            var submitted = rows.Count(r => r.Submission != null);
            var marksList = rows.Where(r => r.Submission?.Marks != null).Select(r => (double)r.Submission.Marks.Value).ToList();

            var model = new StudentDashboardViewModel
            {
                TotalAssignments = rows.Count,
                SubmittedCount = submitted,
                PendingCount = rows.Count - submitted,
                AverageMarks = marksList.Any() ? marksList.Average() : null,
                UpcomingAssignment = rows
                    .Where(r => r.Submission == null && r.Assignment.DueDate >= DateTime.Today)
                    .OrderBy(r => r.Assignment.DueDate)
                    .Select(r => r.Assignment)
                    .FirstOrDefault(),
                Rows = rows
            };

            return View(model);
        }

        // GET: /Student/Assignments
        public async Task<IActionResult> Assignments(string search, int page = 1)
        {
            const int pageSize = 5;
            var studentId = StudentId;

            var query = _context.Assignments
                .Include(a => a.Submissions.Where(s => s.StudentId == studentId))
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(a => a.Title.Contains(search) || a.Subject.Contains(search));
            }

            var total = await query.CountAsync();

            var assignments = await query
                .OrderBy(a => a.DueDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            ViewBag.StudentId = studentId;

            return View(assignments);
        }

        // GET: /Student/DownloadAssignmentFile/5
        public async Task<IActionResult> DownloadAssignmentFile(int id)
        {
            var assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.AssignmentId == id);
            if (assignment == null) return NotFound();

            return DownloadPhysicalFile(assignment.FilePath);
        }

        // GET: /Student/Submit/5 (assignment id)
        public async Task<IActionResult> Submit(int id)
        {
            var assignment = await _context.Assignments
                .Include(a => a.Submissions.Where(s => s.StudentId == StudentId))
                .FirstOrDefaultAsync(a => a.AssignmentId == id);

            if (assignment == null) return NotFound();

            ViewBag.ExistingSubmission = assignment.Submissions.FirstOrDefault();
            return View(assignment);
        }

        // POST: /Student/Submit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(int id, IFormFile file)
        {
            var assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.AssignmentId == id);
            if (assignment == null) return NotFound();

            if (file == null)
            {
                ModelState.AddModelError(string.Empty, "Please choose a file to submit.");
            }
            else
            {
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!AllowedExtensions.Contains(ext))
                {
                    ModelState.AddModelError(string.Empty, "Only PDF, DOC and DOCX files are allowed.");
                }
                if (file.Length > MaxFileSize)
                {
                    ModelState.AddModelError(string.Empty, "File size must not exceed 20 MB.");
                }
            }

            var existing = await _context.Submissions
                .FirstOrDefaultAsync(s => s.AssignmentId == id && s.StudentId == StudentId);

            // A student may only replace their submission before the deadline.
            if (existing != null && DateTime.Today > assignment.DueDate)
            {
                ModelState.AddModelError(string.Empty, "The deadline has passed. You can no longer replace your submission.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ExistingSubmission = existing;
                return View(assignment);
            }

            var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads", "submissions");
            Directory.CreateDirectory(uploadsRoot);
            var uniqueName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(uploadsRoot, uniqueName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = $"/uploads/submissions/{uniqueName}";
            var now = DateTime.Now;
            var isLate = DateTime.Today > assignment.DueDate;

            if (existing != null)
            {
                // Replace: remove old file, keep the same submission record (marks/remarks reset).
                DeletePhysicalFile(existing.FilePath);
                existing.FilePath = relativePath;
                existing.SubmissionDate = now;
                existing.IsLate = isLate;
                existing.Marks = null;
                existing.Remarks = null;
            }
            else
            {
                var submission = new Submission
                {
                    AssignmentId = id,
                    StudentId = StudentId,
                    SubmissionDate = now,
                    FilePath = relativePath,
                    IsLate = isLate
                };
                _context.Submissions.Add(submission);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Assignment submitted successfully.";
            return RedirectToAction(nameof(Assignments));
        }

        // GET: /Student/Marks
        public async Task<IActionResult> Marks()
        {
            var submissions = await _context.Submissions
                .Include(s => s.Assignment)
                .Where(s => s.StudentId == StudentId)
                .OrderByDescending(s => s.SubmissionDate)
                .ToListAsync();

            return View(submissions);
        }

        // ---------- Helpers ----------

        private void DeletePhysicalFile(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return;
            var fullPath = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }

        private IActionResult DownloadPhysicalFile(string relativePath)
        {
            var fullPath = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound("File not found on server.");
            }

            var fileName = Path.GetFileName(fullPath);
            return PhysicalFile(fullPath, "application/octet-stream", fileName);
        }
    }
}
