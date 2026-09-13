using AhmedOS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AhmedOS.Infrastructure.Data;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class AhmedOSDbContext : IdentityDbContext<ApplicationUser>
{
    public AhmedOSDbContext(DbContextOptions<AhmedOSDbContext> options) : base(options) { }

    // Core
    public DbSet<TodoTask> TodoTasks => Set<TodoTask>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<TaskTag> TaskTags => Set<TaskTag>();
    public DbSet<NoteTag> NoteTags => Set<NoteTag>();
    public DbSet<TaskReminder> TaskReminders => Set<TaskReminder>();

    // Projects
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectMilestone> ProjectMilestones => Set<ProjectMilestone>();

    // Goals
    public DbSet<Goal> Goals => Set<Goal>();
    public DbSet<GoalMilestone> GoalMilestones => Set<GoalMilestone>();

    // Habits
    public DbSet<Habit> Habits => Set<Habit>();
    public DbSet<HabitEntry> HabitEntries => Set<HabitEntry>();

    // Notes
    public DbSet<Note> Notes => Set<Note>();

    // Calendar
    public DbSet<CalendarEvent> CalendarEvents => Set<CalendarEvent>();

    // Study
    public DbSet<StudyTopic> StudyTopics => Set<StudyTopic>();
    public DbSet<StudySession> StudySessions => Set<StudySession>();
    public DbSet<StudyResource> StudyResources => Set<StudyResource>();

    // University
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseGrade> CourseGrades => Set<CourseGrade>();

    // DEPI
    public DbSet<DEPIModule> DEPIModules => Set<DEPIModule>();
    public DbSet<DEPISession> DEPISessions => Set<DEPISession>();

    // Career
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();
    public DbSet<JobInterview> JobInterviews => Set<JobInterview>();

    // Skills
    public DbSet<Skill> Skills => Set<Skill>();

    // Time tracking
    public DbSet<FocusSession> FocusSessions => Set<FocusSession>();
    public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();

    // System
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<UserSettings> UserSettings => Set<UserSettings>();
    public DbSet<DailyReview> DailyReviews => Set<DailyReview>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // TaskTag join table
        builder.Entity<TaskTag>(e =>
        {
            e.HasKey(tt => new { tt.TaskId, tt.TagId });
            e.HasOne(tt => tt.Task).WithMany(t => t.TaskTags).HasForeignKey(tt => tt.TaskId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(tt => tt.Tag).WithMany(t => t.TaskTags).HasForeignKey(tt => tt.TagId).OnDelete(DeleteBehavior.Cascade);
        });

        // NoteTag join table
        builder.Entity<NoteTag>(e =>
        {
            e.HasKey(nt => new { nt.NoteId, nt.TagId });
            e.HasOne(nt => nt.Note).WithMany(n => n.NoteTags).HasForeignKey(nt => nt.NoteId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(nt => nt.Tag).WithMany(t => t.NoteTags).HasForeignKey(nt => nt.TagId).OnDelete(DeleteBehavior.Cascade);
        });

        // TodoTask self-referencing (parent/subtasks)
        builder.Entity<TodoTask>(e =>
        {
            e.HasOne(t => t.ParentTask).WithMany(t => t.SubTasks).HasForeignKey(t => t.ParentTaskId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(t => t.Category).WithMany(c => c.Tasks).HasForeignKey(t => t.CategoryId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(t => t.Project).WithMany(p => p.Tasks).HasForeignKey(t => t.ProjectId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(t => t.Goal).WithMany(g => g.Tasks).HasForeignKey(t => t.GoalId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(t => t.Course).WithMany(c => c.Tasks).HasForeignKey(t => t.CourseId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(t => t.DEPIModule).WithMany(d => d.Tasks).HasForeignKey(t => t.DEPIModuleId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(t => t.StudyTopic).WithMany(s => s.Tasks).HasForeignKey(t => t.StudyTopicId).OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(t => t.UserId);
            e.HasIndex(t => t.Status);
            e.HasIndex(t => t.DueDate);
            e.HasIndex(t => t.Priority);
        });

        // Goal self-referencing (parent/sub-goals)
        builder.Entity<Goal>(e =>
        {
            e.HasOne(g => g.ParentGoal).WithMany(g => g.SubGoals).HasForeignKey(g => g.ParentGoalId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(g => g.UserId);
        });

        // Skill self-referencing
        builder.Entity<Skill>(e =>
        {
            e.HasOne(s => s.ParentSkill).WithMany(s => s.SubSkills).HasForeignKey(s => s.ParentSkillId).OnDelete(DeleteBehavior.Restrict);
        });

        // Project
        builder.Entity<Project>(e =>
        {
            e.HasOne(p => p.Category).WithMany(c => c.Projects).HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(p => p.UserId);
        });

        // Note
        builder.Entity<Note>(e =>
        {
            e.HasOne(n => n.Category).WithMany(c => c.Notes).HasForeignKey(n => n.CategoryId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(n => n.Project).WithMany(p => p.Notes).HasForeignKey(n => n.ProjectId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(n => n.Course).WithMany(c => c.Notes).HasForeignKey(n => n.CourseId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(n => n.Goal).WithMany().HasForeignKey(n => n.GoalId).OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(n => n.UserId);
        });

        // FocusSession
        builder.Entity<FocusSession>(e =>
        {
            e.HasOne(f => f.Task).WithMany().HasForeignKey(f => f.TaskId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(f => f.Project).WithMany().HasForeignKey(f => f.ProjectId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(f => f.StudyTopic).WithMany().HasForeignKey(f => f.StudyTopicId).OnDelete(DeleteBehavior.SetNull);
        });

        // TimeEntry
        builder.Entity<TimeEntry>(e =>
        {
            e.HasOne(t => t.Task).WithMany(t => t.TimeEntries).HasForeignKey(t => t.TaskId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(t => t.Project).WithMany(p => p.TimeEntries).HasForeignKey(t => t.ProjectId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(t => t.StudyTopic).WithMany().HasForeignKey(t => t.StudyTopicId).OnDelete(DeleteBehavior.SetNull);
        });

        // Calendar event
        builder.Entity<CalendarEvent>(e =>
        {
            e.HasOne(c => c.Project).WithMany().HasForeignKey(c => c.ProjectId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(c => c.Course).WithMany(c => c.Events).HasForeignKey(c => c.CourseId).OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(c => c.UserId);
            e.HasIndex(c => c.StartTime);
        });

        // Career
        builder.Entity<JobApplication>(e =>
        {
            e.HasOne(j => j.Company).WithMany(c => c.Applications).HasForeignKey(j => j.CompanyId).OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(j => j.UserId);
        });

        // Soft delete filter for all BaseEntity types
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(AhmedOS.Domain.Common.BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                var property = System.Linq.Expressions.Expression.Property(parameter, nameof(AhmedOS.Domain.Common.BaseEntity.IsDeleted));
                var falseConstant = System.Linq.Expressions.Expression.Constant(false);
                var condition = System.Linq.Expressions.Expression.Equal(property, falseConstant);
                var lambda = System.Linq.Expressions.Expression.Lambda(condition, parameter);
                builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<AhmedOS.Domain.Common.BaseEntity>();
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
