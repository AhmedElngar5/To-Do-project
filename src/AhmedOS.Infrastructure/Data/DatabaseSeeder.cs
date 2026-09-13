using AhmedOS.Domain.Entities;
using AhmedOS.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AhmedOS.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AhmedOSDbContext context, UserManager<ApplicationUser> userManager)
    {
        // Create default user if not exists
        var adminEmail = Environment.GetEnvironmentVariable("INITIAL_ADMIN_EMAIL") ?? "ahmed@ahmeddos.local";
        var adminPassword = Environment.GetEnvironmentVariable("INITIAL_ADMIN_PASSWORD") ?? "Ahmed@OS2024!";

        var user = await userManager.FindByEmailAsync(adminEmail);
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Ahmed Hany Kamal El Nagar",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(user, adminPassword);
        }

        var userId = user.Id;

        // Seed categories
        if (!await context.Categories.AnyAsync())
        {
            var categories = new List<Category>
            {
                new() { Name = "University", Icon = "🎓", Color = "#6366f1", SortOrder = 1, IsSystem = true, UserId = userId },
                new() { Name = "DEPI", Icon = "📋", Color = "#8b5cf6", SortOrder = 2, IsSystem = true, UserId = userId },
                new() { Name = "Study", Icon = "📚", Color = "#3b82f6", SortOrder = 3, IsSystem = true, UserId = userId },
                new() { Name = "C#", Icon = "💜", Color = "#9333ea", SortOrder = 4, IsSystem = true, UserId = userId },
                new() { Name = ".NET", Icon = "🟣", Color = "#7c3aed", SortOrder = 5, IsSystem = true, UserId = userId },
                new() { Name = "AI", Icon = "🤖", Color = "#06b6d4", SortOrder = 6, IsSystem = true, UserId = userId },
                new() { Name = "Graduation Project", Icon = "🎯", Color = "#f59e0b", SortOrder = 7, IsSystem = true, UserId = userId },
                new() { Name = "Wasel", Icon = "🔗", Color = "#f97316", SortOrder = 8, IsSystem = true, UserId = userId },
                new() { Name = "Career", Icon = "💼", Color = "#22c55e", SortOrder = 9, IsSystem = true, UserId = userId },
                new() { Name = "Job Search", Icon = "🔍", Color = "#10b981", SortOrder = 10, IsSystem = true, UserId = userId },
                new() { Name = "Personal Projects", Icon = "🛠️", Color = "#0ea5e9", SortOrder = 11, IsSystem = true, UserId = userId },
                new() { Name = "Personal", Icon = "👤", Color = "#64748b", SortOrder = 12, IsSystem = true, UserId = userId },
                new() { Name = "Planning", Icon = "📅", Color = "#14b8a6", SortOrder = 13, IsSystem = true, UserId = userId },
                new() { Name = "Other", Icon = "📌", Color = "#94a3b8", SortOrder = 14, IsSystem = true, UserId = userId },
            };
            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
        }

        // Seed default tags
        if (!await context.Tags.AnyAsync())
        {
            var tags = new List<Tag>
            {
                new() { Name = "csharp", Color = "#9333ea", UserId = userId },
                new() { Name = "dotnet", Color = "#7c3aed", UserId = userId },
                new() { Name = "depi", Color = "#8b5cf6", UserId = userId },
                new() { Name = "university", Color = "#6366f1", UserId = userId },
                new() { Name = "graduation-project", Color = "#f59e0b", UserId = userId },
                new() { Name = "career", Color = "#22c55e", UserId = userId },
                new() { Name = "study", Color = "#3b82f6", UserId = userId },
                new() { Name = "urgent", Color = "#ef4444", UserId = userId },
                new() { Name = "ai", Color = "#06b6d4", UserId = userId },
                new() { Name = "backend", Color = "#7c3aed", UserId = userId },
                new() { Name = "frontend", Color = "#0ea5e9", UserId = userId },
            };
            context.Tags.AddRange(tags);
            await context.SaveChangesAsync();
        }

        // Seed projects
        if (!await context.Projects.AnyAsync())
        {
            var gradCat = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Graduation Project");
            var personalCat = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Personal Projects");
            var studyCat = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Study");

            var projects = new List<Project>
            {
                new()
                {
                    Name = "Wasel",
                    Description = "AI-Based Child Family Reunification & Alternative Care Support System",
                    Type = ProjectType.Graduation,
                    Status = ProjectStatus.Active,
                    Priority = Priority.Critical,
                    Technologies = "C#, ASP.NET Core, SQL Server, Angular, AI, NLP, Computer Vision",
                    Color = "#f97316",
                    Icon = "🔗",
                    CategoryId = gradCat?.Id,
                    UserId = userId,
                    Progress = 0
                },
                new()
                {
                    Name = "Personal Portfolio",
                    Description = "Professional portfolio showcasing projects, skills, and experience",
                    Type = ProjectType.Portfolio,
                    Status = ProjectStatus.Planning,
                    Priority = Priority.High,
                    Technologies = "HTML, CSS, JavaScript, .NET",
                    Color = "#0ea5e9",
                    Icon = "🌐",
                    CategoryId = personalCat?.Id,
                    UserId = userId,
                    Progress = 0
                },
                new()
                {
                    Name = "Full Stack .NET Learning Roadmap",
                    Description = "Structured learning path toward becoming a Full Stack .NET Developer",
                    Type = ProjectType.Learning,
                    Status = ProjectStatus.Active,
                    Priority = Priority.High,
                    Technologies = "C#, .NET, ASP.NET Core, SQL Server, Angular",
                    Color = "#7c3aed",
                    Icon = "🗺️",
                    CategoryId = studyCat?.Id,
                    UserId = userId,
                    Progress = 0
                },
            };
            context.Projects.AddRange(projects);
            await context.SaveChangesAsync();
        }

        // Seed goals
        if (!await context.Goals.AnyAsync())
        {
            var visionGoal = new Goal
            {
                Title = "Become a professional software engineer",
                Level = GoalLevel.Vision,
                Status = GoalStatus.OnTrack,
                Priority = Priority.Critical,
                UserId = userId,
                WhyItMatters = "My long-term career vision"
            };
            context.Goals.Add(visionGoal);
            await context.SaveChangesAsync();

            var yearlyGoals = new List<Goal>
            {
                new()
                {
                    Title = "Become job-ready as a Full Stack .NET Developer",
                    Level = GoalLevel.Yearly,
                    Status = GoalStatus.OnTrack,
                    Priority = Priority.Critical,
                    ParentGoalId = visionGoal.Id,
                    UserId = userId,
                    WhyItMatters = "Foundation for my career"
                },
                new()
                {
                    Title = "Complete important university milestones",
                    Level = GoalLevel.Yearly,
                    Status = GoalStatus.OnTrack,
                    Priority = Priority.High,
                    ParentGoalId = visionGoal.Id,
                    UserId = userId
                },
                new()
                {
                    Title = "Develop strong C# and ASP.NET Core skills",
                    Level = GoalLevel.Yearly,
                    Status = GoalStatus.OnTrack,
                    Priority = Priority.Critical,
                    ParentGoalId = visionGoal.Id,
                    UserId = userId
                },
                new()
                {
                    Title = "Build professional projects",
                    Level = GoalLevel.Yearly,
                    Status = GoalStatus.OnTrack,
                    Priority = Priority.High,
                    ParentGoalId = visionGoal.Id,
                    UserId = userId
                },
                new()
                {
                    Title = "Complete the Wasel graduation project",
                    Level = GoalLevel.Yearly,
                    Status = GoalStatus.OnTrack,
                    Priority = Priority.Critical,
                    ParentGoalId = visionGoal.Id,
                    UserId = userId
                },
                new()
                {
                    Title = "Build a professional portfolio",
                    Level = GoalLevel.Yearly,
                    Status = GoalStatus.NotStarted,
                    Priority = Priority.High,
                    ParentGoalId = visionGoal.Id,
                    UserId = userId
                },
                new()
                {
                    Title = "Start and maintain a career application pipeline",
                    Level = GoalLevel.Yearly,
                    Status = GoalStatus.NotStarted,
                    Priority = Priority.Medium,
                    ParentGoalId = visionGoal.Id,
                    UserId = userId
                },
            };
            context.Goals.AddRange(yearlyGoals);
            await context.SaveChangesAsync();
        }

        // Seed study topics
        if (!await context.StudyTopics.AnyAsync())
        {
            var topics = new List<StudyTopic>
            {
                new() { Name = "C#", Icon = "💜", Status = StudyStatus.Learning, Difficulty = Difficulty.Medium, Confidence = ConfidenceLevel.Medium, Progress = 60, UserId = userId },
                new() { Name = "Java", Icon = "☕", Status = StudyStatus.Learning, Difficulty = Difficulty.Medium, Confidence = ConfidenceLevel.Low, Progress = 30, UserId = userId },
                new() { Name = "Data Structures", Icon = "🏗️", Status = StudyStatus.Learning, Difficulty = Difficulty.Hard, Confidence = ConfidenceLevel.Medium, Progress = 45, UserId = userId },
                new() { Name = "Algorithms", Icon = "⚡", Status = StudyStatus.Learning, Difficulty = Difficulty.Hard, Confidence = ConfidenceLevel.Low, Progress = 35, UserId = userId },
                new() { Name = "OOP", Icon = "🔲", Status = StudyStatus.Practicing, Difficulty = Difficulty.Medium, Confidence = ConfidenceLevel.Medium, Progress = 65, UserId = userId },
                new() { Name = ".NET", Icon = "🟣", Status = StudyStatus.Learning, Difficulty = Difficulty.Medium, Confidence = ConfidenceLevel.Medium, Progress = 50, UserId = userId },
                new() { Name = "ASP.NET Core", Icon = "🌐", Status = StudyStatus.Learning, Difficulty = Difficulty.Hard, Confidence = ConfidenceLevel.Low, Progress = 40, UserId = userId },
                new() { Name = "SQL Server", Icon = "🗃️", Status = StudyStatus.Learning, Difficulty = Difficulty.Medium, Confidence = ConfidenceLevel.Medium, Progress = 50, UserId = userId },
                new() { Name = "Entity Framework Core", Icon = "📊", Status = StudyStatus.Learning, Difficulty = Difficulty.Medium, Confidence = ConfidenceLevel.Low, Progress = 35, UserId = userId },
                new() { Name = "Git & GitHub", Icon = "🔀", Status = StudyStatus.Practicing, Difficulty = Difficulty.Easy, Confidence = ConfidenceLevel.Medium, Progress = 60, UserId = userId },
                new() { Name = "Software Engineering", Icon = "📐", Status = StudyStatus.Learning, Difficulty = Difficulty.Medium, Confidence = ConfidenceLevel.Low, Progress = 40, UserId = userId },
                new() { Name = "REST APIs", Icon = "🔌", Status = StudyStatus.Learning, Difficulty = Difficulty.Medium, Confidence = ConfidenceLevel.Low, Progress = 35, UserId = userId },
                new() { Name = "AI & Machine Learning", Icon = "🤖", Status = StudyStatus.Learning, Difficulty = Difficulty.Advanced, Confidence = ConfidenceLevel.Low, Progress = 25, UserId = userId },
                new() { Name = "NLP", Icon = "💬", Status = StudyStatus.NotStarted, Difficulty = Difficulty.Advanced, Confidence = ConfidenceLevel.None, Progress = 10, UserId = userId },
                new() { Name = "Computer Vision", Icon = "👁️", Status = StudyStatus.NotStarted, Difficulty = Difficulty.Advanced, Confidence = ConfidenceLevel.None, Progress = 5, UserId = userId },
                new() { Name = "Angular", Icon = "🅰️", Status = StudyStatus.NotStarted, Difficulty = Difficulty.Hard, Confidence = ConfidenceLevel.None, Progress = 0, UserId = userId },
            };
            context.StudyTopics.AddRange(topics);
            await context.SaveChangesAsync();
        }

        // Seed skills for the roadmap
        if (!await context.Skills.AnyAsync())
        {
            var skills = new List<Skill>
            {
                new() { Name = "Programming Fundamentals", Status = StudyStatus.Practicing, Confidence = ConfidenceLevel.Medium, Progress = 70, SortOrder = 1, UserId = userId },
                new() { Name = "C#", Status = StudyStatus.Learning, Confidence = ConfidenceLevel.Medium, Progress = 60, SortOrder = 2, UserId = userId },
                new() { Name = "OOP", Status = StudyStatus.Practicing, Confidence = ConfidenceLevel.Medium, Progress = 65, SortOrder = 3, UserId = userId },
                new() { Name = "Data Structures", Status = StudyStatus.Learning, Confidence = ConfidenceLevel.Low, Progress = 45, SortOrder = 4, UserId = userId },
                new() { Name = "Algorithms", Status = StudyStatus.Learning, Confidence = ConfidenceLevel.Low, Progress = 35, SortOrder = 5, UserId = userId },
                new() { Name = "SQL", Status = StudyStatus.Learning, Confidence = ConfidenceLevel.Medium, Progress = 50, SortOrder = 6, UserId = userId },
                new() { Name = "Git", Status = StudyStatus.Practicing, Confidence = ConfidenceLevel.Medium, Progress = 60, SortOrder = 7, UserId = userId },
                new() { Name = ".NET", Status = StudyStatus.Learning, Confidence = ConfidenceLevel.Low, Progress = 50, SortOrder = 8, UserId = userId },
                new() { Name = "ASP.NET Core", Status = StudyStatus.Learning, Confidence = ConfidenceLevel.Low, Progress = 40, SortOrder = 9, UserId = userId },
                new() { Name = "REST APIs", Status = StudyStatus.Learning, Confidence = ConfidenceLevel.Low, Progress = 35, SortOrder = 10, UserId = userId },
                new() { Name = "Entity Framework Core", Status = StudyStatus.Learning, Confidence = ConfidenceLevel.Low, Progress = 35, SortOrder = 11, UserId = userId },
                new() { Name = "Authentication", Status = StudyStatus.NotStarted, Confidence = ConfidenceLevel.None, Progress = 15, SortOrder = 12, UserId = userId },
                new() { Name = "Testing", Status = StudyStatus.NotStarted, Confidence = ConfidenceLevel.None, Progress = 10, SortOrder = 13, UserId = userId },
                new() { Name = "Software Architecture", Status = StudyStatus.NotStarted, Confidence = ConfidenceLevel.None, Progress = 20, SortOrder = 14, UserId = userId },
                new() { Name = "Deployment", Status = StudyStatus.NotStarted, Confidence = ConfidenceLevel.None, Progress = 5, SortOrder = 15, UserId = userId },
                new() { Name = "Angular", Status = StudyStatus.NotStarted, Confidence = ConfidenceLevel.None, Progress = 0, SortOrder = 16, UserId = userId },
                new() { Name = "Full Stack", Status = StudyStatus.NotStarted, Confidence = ConfidenceLevel.None, Progress = 0, SortOrder = 17, UserId = userId },
                new() { Name = "Portfolio", Status = StudyStatus.NotStarted, Confidence = ConfidenceLevel.None, Progress = 0, SortOrder = 18, UserId = userId },
                new() { Name = "Interview Preparation", Status = StudyStatus.NotStarted, Confidence = ConfidenceLevel.None, Progress = 0, SortOrder = 19, UserId = userId },
            };
            context.Skills.AddRange(skills);
            await context.SaveChangesAsync();
        }

        // Seed default habits
        if (!await context.Habits.AnyAsync())
        {
            var habits = new List<Habit>
            {
                new() { Name = "Study", Icon = "📚", Color = "#3b82f6", Schedule = RecurrenceType.Daily, TargetCount = 1, TargetUnit = "session", UserId = userId },
                new() { Name = "Coding", Icon = "💻", Color = "#7c3aed", Schedule = RecurrenceType.Daily, TargetCount = 1, TargetUnit = "session", UserId = userId },
                new() { Name = "Daily Planning", Icon = "📅", Color = "#14b8a6", Schedule = RecurrenceType.Daily, TargetCount = 1, TargetUnit = "session", UserId = userId },
                new() { Name = "Daily Review", Icon = "📝", Color = "#f59e0b", Schedule = RecurrenceType.Daily, TargetCount = 1, TargetUnit = "session", UserId = userId },
                new() { Name = "Exercise", Icon = "🏃", Color = "#22c55e", Schedule = RecurrenceType.Daily, TargetCount = 1, TargetUnit = "session", UserId = userId },
                new() { Name = "Reading", Icon = "📖", Color = "#06b6d4", Schedule = RecurrenceType.Daily, TargetCount = 1, TargetUnit = "session", UserId = userId },
                new() { Name = "Project Work", Icon = "🛠️", Color = "#f97316", Schedule = RecurrenceType.Daily, TargetCount = 1, TargetUnit = "session", UserId = userId },
            };
            context.Habits.AddRange(habits);
            await context.SaveChangesAsync();
        }

        // Seed user settings
        if (!await context.UserSettings.AnyAsync(us => us.UserId == userId))
        {
            context.UserSettings.Add(new UserSettings { UserId = userId });
            await context.SaveChangesAsync();
        }

        // Seed some sample tasks
        if (!await context.TodoTasks.AnyAsync())
        {
            var today = DateTime.UtcNow.Date;
            var studyCat = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Study");
            var depiCat = await context.Categories.FirstOrDefaultAsync(c => c.Name == "DEPI");
            var gradCat = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Graduation Project");
            var careerCat = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Career");
            var wasel = await context.Projects.FirstOrDefaultAsync(p => p.Name == "Wasel");

            var tasks = new List<TodoTask>
            {
                new()
                {
                    Title = "Study ASP.NET Core Web APIs",
                    Description = "Focus on controllers, routing, and dependency injection",
                    Status = TodoTaskStatus.Planned,
                    Priority = Priority.High,
                    DueDate = today.AddDays(1),
                    EstimatedMinutes = 90,
                    CategoryId = studyCat?.Id,
                    UserId = userId,
                    Source = "seed"
                },
                new()
                {
                    Title = "Complete DEPI Module Assignment",
                    Description = "Submit the C# assignment for the current DEPI module",
                    Status = TodoTaskStatus.Planned,
                    Priority = Priority.Critical,
                    DueDate = today.AddDays(2),
                    EstimatedMinutes = 120,
                    CategoryId = depiCat?.Id,
                    UserId = userId,
                    Source = "seed"
                },
                new()
                {
                    Title = "Work on Wasel database design",
                    Description = "Design the core database schema for the child reunification system",
                    Status = TodoTaskStatus.InProgress,
                    Priority = Priority.High,
                    DueDate = today.AddDays(5),
                    EstimatedMinutes = 180,
                    CategoryId = gradCat?.Id,
                    ProjectId = wasel?.Id,
                    UserId = userId,
                    Source = "seed"
                },
                new()
                {
                    Title = "Practice Data Structures problems",
                    Description = "Solve 3 problems on arrays and linked lists",
                    Status = TodoTaskStatus.Planned,
                    Priority = Priority.Medium,
                    DueDate = today.AddDays(1),
                    EstimatedMinutes = 60,
                    CategoryId = studyCat?.Id,
                    UserId = userId,
                    Source = "seed"
                },
                new()
                {
                    Title = "Update CV and LinkedIn profile",
                    Description = "Add recent projects and skills",
                    Status = TodoTaskStatus.Planned,
                    Priority = Priority.Medium,
                    DueDate = today.AddDays(7),
                    EstimatedMinutes = 45,
                    CategoryId = careerCat?.Id,
                    UserId = userId,
                    Source = "seed"
                },
                new()
                {
                    Title = "Review OOP concepts",
                    Description = "Review SOLID principles and design patterns",
                    Status = TodoTaskStatus.Planned,
                    Priority = Priority.Medium,
                    DueDate = today.AddDays(3),
                    EstimatedMinutes = 60,
                    CategoryId = studyCat?.Id,
                    UserId = userId,
                    Source = "seed"
                },
            };
            context.TodoTasks.AddRange(tasks);
            await context.SaveChangesAsync();
        }
    }
}
