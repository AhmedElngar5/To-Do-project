# 🗄️ Database Documentation

Ahmed OS utilizes **Microsoft SQL Server** orchestrated by **Entity Framework Core 10**.

---

## 📋 Entity Groups

The database contains 28 entities grouped into 8 domains:

### 1. Productivity & Tasks
* `TodoTasks` — Core task items with status, priority, estimated effort, score, and relations.
* `Categories` — Colored grouping categories (University, DEPI, Wasel, Career, Personal).
* `Tags` — Fast labeling system for multi-dimensional filtering.
* `TaskReminders` — Scheduled alerts and reminder triggers.

### 2. Projects & Goals
* `Projects` — High-level initiatives (Graduation, Portfolio, Learning).
* `ProjectMilestones` — Sequential delivery targets for each project.
* `Goals` — Hierarchical goal structure (Vision → Yearly → Quarterly → Monthly).

### 3. Academic & University
* `Courses` — Academic courses, credit hours, professors, and grade targets.
* `CourseGrades` — Individual exam, quiz, assignment, and practical grades.

### 4. DEPI (Digital Egypt Pioneers Initiative)
* `DEPIModules` — Training modules with progress and completion tracking.
* `DEPISessions` — Scheduled live classes, dates, and attendance verification.

### 5. Learning & Study
* `StudyTopics` — Subjects with confidence scoring and spaced repetition parameters.
* `StudySessions` — Historical logs of focused study periods.
* `Skills` — Technical competencies aligned with .NET developer roadmap.

### 6. Career
* `Companies` — Target employers and market intelligence.
* `JobApplications` — Job hunting pipeline from applied to offer.
* `Interviews` — Interview scheduling, format, and feedback logs.

### 7. Habits & Focus
* `Habits` — Routine tracking with daily frequencies and streaks.
* `HabitEntries` — Daily boolean completions.
* `FocusSessions` — Pomodoro logs with duration and interruption notes.
* `TimeEntries` — Billable/actual duration logs.

### 8. System & Settings
* `Notifications` — In-app notification queue.
* `UserSettings` — Work hours, theme, and AI configuration.
* `AuditLogs` — Change history for critical updates.
* `DailyReviews` — End-of-day reflections and productivity scoring.
* `Notes` — Knowledge base items with markdown support.

---

## 🔒 Security & Data Isolation
* All entities inherit from `BaseEntity` which includes `CreatedAt`, `UpdatedAt`, and `IsDeleted`.
* **Global Query Filters**: EF Core automatically filters out soft-deleted records (`!IsDeleted`).
* **Tenant Scoping**: All user data is tied to `UserId` to ensure complete privacy and prevent cross-user leakage.
