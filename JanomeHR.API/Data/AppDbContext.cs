using JanomeHR.Shared.Entities;
using JanomeHR.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace JanomeHR.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<JobPosting> JobPostings => Set<JobPosting>();
    public DbSet<Application> Applications => Set<Application>();
    public DbSet<ApplicationDocument> ApplicationDocuments => Set<ApplicationDocument>();
    public DbSet<ApplicationNote> ApplicationNotes => Set<ApplicationNote>();
    public DbSet<Interview> Interviews => Set<Interview>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        // ── User ──────────────────────────────────
        mb.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Username).HasMaxLength(50).IsRequired();
            e.Property(x => x.Email).HasMaxLength(100).IsRequired();
            e.Property(x => x.FullName).HasMaxLength(100).IsRequired();
            e.Property(x => x.Role).HasConversion<string>();
            e.HasIndex(x => x.Username).IsUnique();
            e.HasIndex(x => x.Email).IsUnique();
        });

        // ── Department ────────────────────────────
        mb.Entity<Department>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
        });

        // ── JobPosting ────────────────────────────
        mb.Entity<JobPosting>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).HasMaxLength(150).IsRequired();
            e.Property(x => x.Status).HasConversion<string>();
            e.Property(x => x.SalaryMin).HasColumnType("decimal(10,2)");
            e.Property(x => x.SalaryMax).HasColumnType("decimal(10,2)");

            e.HasOne(x => x.Department)
             .WithMany(x => x.JobPostings)
             .HasForeignKey(x => x.DepartmentId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.CreatedByUser)
             .WithMany(x => x.JobPostings)
             .HasForeignKey(x => x.CreatedBy)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ── Application ───────────────────────────
        mb.Entity<Application>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.ReferenceCode).HasMaxLength(20).IsRequired();
            e.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            e.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(20).IsRequired();
            e.Property(x => x.Email).HasMaxLength(100);
            e.Property(x => x.Status).HasConversion<string>();
            e.Property(x => x.Source).HasConversion<string>();
            e.Property(x => x.EducationLevel).HasConversion<string>();
            e.Property(x => x.SalaryExpected).HasColumnType("decimal(10,2)");
            e.HasIndex(x => x.ReferenceCode).IsUnique();

            e.HasOne(x => x.JobPosting)
             .WithMany(x => x.Applications)
             .HasForeignKey(x => x.JobPostingId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ── ApplicationDocument ───────────────────
        mb.Entity<ApplicationDocument>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.FileName).HasMaxLength(255).IsRequired();
            e.Property(x => x.DocumentType).HasConversion<string>();

            e.HasOne(x => x.Application)
             .WithMany(x => x.Documents)
             .HasForeignKey(x => x.ApplicationId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── ApplicationNote ───────────────────────
        mb.Entity<ApplicationNote>(e =>
        {
            e.HasKey(x => x.Id);

            e.HasOne(x => x.Application)
             .WithMany(x => x.Notes)
             .HasForeignKey(x => x.ApplicationId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.CreatedByUser)
             .WithMany(x => x.ApplicationNotes)
             .HasForeignKey(x => x.CreatedBy)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ── Interview ─────────────────────────────
        mb.Entity<Interview>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Type).HasConversion<string>();
            e.Property(x => x.Status).HasConversion<string>();
            e.Property(x => x.Result).HasConversion<string>();

            e.HasOne(x => x.Application)
             .WithMany(x => x.Interviews)
             .HasForeignKey(x => x.ApplicationId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Interviewer)
             .WithMany(x => x.Interviews)
             .HasForeignKey(x => x.InterviewerId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ── Notification ──────────────────────────
        mb.Entity<Notification>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Recipient).HasMaxLength(100).IsRequired();
            e.Property(x => x.Type).HasConversion<string>();
            e.Property(x => x.Event).HasConversion<string>();
            e.Property(x => x.Status).HasConversion<string>();

            e.HasOne(x => x.Application)
             .WithMany(x => x.Notifications)
             .HasForeignKey(x => x.ApplicationId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── Seed Data ─────────────────────────────
        mb.Entity<Department>().HasData(
            new Department { Id = 1, Name = "ฝ่ายผลิต", Description = "Production Department" },
            new Department { Id = 2, Name = "ฝ่ายควบคุมคุณภาพ", Description = "Quality Control" },
            new Department { Id = 3, Name = "ฝ่าย Logistics", Description = "Warehouse & Logistics" },
            new Department { Id = 4, Name = "ฝ่ายซ่อมบำรุง", Description = "Maintenance" },
            new Department { Id = 5, Name = "ฝ่าย HR", Description = "Human Resources" },
            new Department { Id = 6, Name = "ฝ่ายธุรการ", Description = "Administration" }
        );
    }
}