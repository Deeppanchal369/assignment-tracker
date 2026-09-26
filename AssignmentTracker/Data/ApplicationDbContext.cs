using Microsoft.EntityFrameworkCore;
using AssignmentTracker.Models;

namespace AssignmentTracker.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Submission> Submissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Teacher>()
                .HasIndex(t => t.Email)
                .IsUnique();

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.Email)
                .IsUnique();

            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Teacher)
                .WithMany(t => t.Assignments)
                .HasForeignKey(a => a.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Submission>()
                .HasOne(s => s.Assignment)
                .WithMany(a => a.Submissions)
                .HasForeignKey(s => s.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Submission>()
                .HasOne(s => s.Student)
                .WithMany(st => st.Submissions)
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // A student can submit only once per assignment (replaced, not duplicated)
            modelBuilder.Entity<Submission>()
                .HasIndex(s => new { s.AssignmentId, s.StudentId })
                .IsUnique();
        }
    }
}
