using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssignmentTracker.Data;
using AssignmentTracker.Filters;
using AssignmentTracker.Models;
using AssignmentTracker.Models.ViewModels;

namespace AssignmentTracker.Controllers
{
    [SessionAuthorize("Teacher")]
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private const long MaxFileSize = 20 * 1024 * 1024; // 20 MB
        private static readonly string[] AllowedExtensions = { ".pdf", ".doc", ".docx" };

        public TeacherController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        private int TeacherId => HttpContext.Session.GetInt32("UserId") ?? 0;

        // GET: /Teacher/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var teacherId = TeacherId;

            var totalStudents = await _context.Students.CountAsync();
            var assignments = await _context.Assignments
                .Where(a => a.TeacherId == teacherId)
                .Include(a => a.Submissions)
                .ToListAsync();

            var totalAssignments = assignments.Count;
            var totalSubmissions = assignments.Sum(a => a.Submissions.Count);
            var expectedSubmissions = totalAssignments * (totalStudents == 0 ? 1 : totalStudents);
            var pending = Math.Max(expectedSubmissions - totalSubmissions, 0);
            var late = assignments.Sum(a => a.Submissions.Count(s => s.IsLate));

            var model = new TeacherDashboardViewModel
            {
                TotalStudents = totalStudents,
                TotalAssignments = totalAssignments,
                PendingSubmissions = pending,
                LateSubmissions = late,
                RecentAssignments = assignments.OrderByDescending(a => a.UploadDate).Take(5).ToList()
            };

            return View(model);
        }

        // GET: /Teacher/Assignments
        public async Task<IActionResult> Assignments(string search, int page = 1)
        {
            const int pageSize = 5;
            var teacherId = TeacherId;

            var query = _context.Assignments
                .Where(a => a.TeacherId == teacherId)
                .Include(a => a.Submissions)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(a => a.Title.Contains(search) || a.Subject.Contains(search));
            }

            var total = await query.CountAsync();

            var assignments = await query
                .OrderByDescending(a => a.UploadDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);

            return View(assignments);
        }

        // GET: /Teacher/CreateAssignment
        public IActionResult CreateAssignment()
        {
            return View(new AssignmentFormViewModel());
        }

        // POST: /Teacher/CreateAssignment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAssignment(AssignmentFormViewModel model)
        {
            if (model.File == null)
            {
                ModelState.AddModelError(nameof(model.File), "Please attach an assignment file.");
            }
            else
            {
                ValidateFile(model.File);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var filePath = await SaveFileAsync(model.File, "assignments");

            var assignment = new Assignment
            {
                Title = model.Title,
                Description = model.Description,
                Subject = model.Subject,
                UploadDate = DateTime.Today,
                DueDate = model.DueDate,
                FilePath = filePath,
                TeacherId = TeacherId
            };

            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Assignment created successfully.";
            return RedirectToAction(nameof(Assignments));
        }

        // GET: /Teacher/EditAssignment/5
        public async Task<IActionResult> EditAssignment(int id)
        {
            var assignment = await _context.Assignments
                .FirstOrDefaultAsync(a => a.AssignmentId == id && a.TeacherId == TeacherId);

            if (assignment == null) return NotFound();

            var model = new AssignmentFormViewModel
            {
                AssignmentId = assignment.AssignmentId,
                Title = assignment.Title,
                Description = assignment.Description,
                Subject = assignment.Subject,
                DueDate = assignment.DueDate,
                ExistingFilePath = assignment.FilePath
            };

            return View(model);
        }

        // POST: /Teacher/EditAssignment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAssignment(int id, AssignmentFormViewModel model)
        {
            if (id != model.AssignmentId) return NotFound();

            if (model.File != null)
            {
                ValidateFile(model.File);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var assignment = await _context.Assignments
                .FirstOrDefaultAsync(a => a.AssignmentId == id && a.TeacherId == TeacherId);

            if (assignment == null) return NotFound();

            assignment.Title = model.Title;
            assignment.Description = model.Description;
            assignment.Subject = model.Subject;
            assignment.DueDate = model.DueDate;

            if (model.File != null)
            {
                DeletePhysicalFile(assignment.FilePath);
                assignment.FilePath = await SaveFileAsync(model.File, "assignments");
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Assignment updated successfully.";
            return RedirectToAction(nameof(Assignments));
        }

        // POST: /Teacher/DeleteAssignment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            var assignment = await _context.Assignments
                .FirstOrDefaultAsync(a => a.AssignmentId == id && a.TeacherId == TeacherId);

            if (assignment == null) return NotFound();

            DeletePhysicalFile(assignment.FilePath);
            _context.Assignments.Remove(assignment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Assignment deleted successfully.";
            return RedirectToAction(nameof(Assignments));
        }

        // GET: /Teacher/ViewAssignment/5
        public async Task<IActionResult> ViewAssignment(int id)
        {
            var assignment = await _context.Assignments
                .Include(a => a.Submissions).ThenInclude(s => s.Student)
                .FirstOrDefaultAsync(a => a.AssignmentId == id && a.TeacherId == TeacherId);

            if (assignment == null) return NotFound();

            return View(assignment);
        }

        // GET: /Teacher/DownloadAssignmentFile/5
        public async Task<IActionResult> DownloadAssignmentFile(int id)
        {
            var assignment = await _context.Assignments
                .FirstOrDefaultAsync(a => a.AssignmentId == id && a.TeacherId == TeacherId);

            if (assignment == null) return NotFound();

            return DownloadPhysicalFile(assignment.FilePath);
        }

        // GET: /Teacher/ViewSubmissions/5  (assignment id)
        public async Task<IActionResult> ViewSubmissions(int id)
        {
            var assignment = await _context.Assignments
                .Include(a => a.Submissions).ThenInclude(s => s.Student)
                .FirstOrDefaultAsync(a => a.AssignmentId == id && a.TeacherId == TeacherId);

            if (assignment == null) return NotFound();

            return View(assignment);
        }

        // GET: /Teacher/DownloadSubmission/5 (submission id)
        public async Task<IActionResult> DownloadSubmission(int id)
        {
            var submission = await _context.Submissions
                .Include(s => s.Assignment)
                .FirstOrDefaultAsync(s => s.SubmissionId == id && s.Assignment.TeacherId == TeacherId);

            if (submission == null) return NotFound();

            return DownloadPhysicalFile(submission.FilePath);
        }

        // GET: /Teacher/EnterMarks/5 (submission id)
        public async Task<IActionResult> EnterMarks(int id)
        {
            var submission = await _context.Submissions
                .Include(s => s.Assignment)
                .Include(s => s.Student)
                .FirstOrDefaultAsync(s => s.SubmissionId == id && s.Assignment.TeacherId == TeacherId);

            if (submission == null) return NotFound();

            return View(submission);
        }

        // POST: /Teacher/EnterMarks/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnterMarks(int id, int? marks, string remarks)
        {
            var submission = await _context.Submissions
                .Include(s => s.Assignment)
                .FirstOrDefaultAsync(s => s.SubmissionId == id && s.Assignment.TeacherId == TeacherId);

            if (submission == null) return NotFound();

            if (marks is < 0 or > 100)
            {
                ModelState.AddModelError(string.Empty, "Marks must be between 0 and 100.");
                await _context.Entry(submission).Reference(s => s.Student).LoadAsync();
                return View(submission);
            }

            submission.Marks = marks;
            submission.Remarks = remarks;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Marks and remarks saved successfully.";
            return RedirectToAction(nameof(ViewSubmissions), new { id = submission.AssignmentId });
        }

        // GET: /Teacher/LateSubmissions
        public async Task<IActionResult> LateSubmissions()
        {
            var submissions = await _context.Submissions
                .Include(s => s.Assignment)
                .Include(s => s.Student)
                .Where(s => s.IsLate && s.Assignment.TeacherId == TeacherId)
                .OrderByDescending(s => s.SubmissionDate)
                .ToListAsync();

            return View(submissions);
        }

        // ---------- Helpers ----------

        private void ValidateFile(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
            {
                ModelState.AddModelError(nameof(AssignmentFormViewModel.File), "Only PDF, DOC and DOCX files are allowed.");
            }
            if (file.Length > MaxFileSize)
            {
                ModelState.AddModelError(nameof(AssignmentFormViewModel.File), "File size must not exceed 20 MB.");
            }
        }

        private async Task<string> SaveFileAsync(IFormFile file, string subFolder)
        {
            var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads", subFolder);
            Directory.CreateDirectory(uploadsRoot);

            var uniqueName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(uploadsRoot, uniqueName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{subFolder}/{uniqueName}";
        }

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
            var contentType = "application/octet-stream";
            return PhysicalFile(fullPath, contentType, fileName);
        }
    }
}
