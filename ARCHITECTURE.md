# 🏛️ Ahmed OS — Architecture Documentation

Ahmed OS is designed using the principles of **Clean Architecture** and **Domain-Driven Design (DDD)**.
The system guarantees clean separation of concerns, high testability, and framework independence for core business rules.

```
                  ┌─────────────────────────────────────┐
                  │            Presentation             │
                  │   (AhmedOS.Web - Razor & Web API)   │
                  └──────────────────┬──────────────────┘
                                     │
                  ┌──────────────────▼──────────────────┐
                  │            Application              │
                  │  (Interfaces, Services, Contracts)  │
                  └──────────────────┬──────────────────┘
                                     │
                  ┌──────────────────▼──────────────────┐
                  │              Domain                 │
                  │ (Entities, Enums, Value Objects)    │
                  └──────────────────▲──────────────────┘
                                     │
                  ┌──────────────────┴──────────────────┐
                  │           Infrastructure            │
                  │  (EF Core, SQL Server, Local AI)    │
                  └─────────────────────────────────────┘
```

---

## Layer Responsibilities

### 1. `AhmedOS.Domain`
* Contains pure business domain logic, domain entities, and enums.
* Has **zero dependencies** on external frameworks or databases.
* Key Entities:
  - `TodoTask`, `Category`, `Project`, `ProjectMilestone`, `Goal`, `Habit`, `HabitEntry`
  - `Course`, `CourseGrade`, `DEPIModule`, `DEPISession`, `StudyTopic`, `Skill`
  - `JobApplication`, `Company`, `Interview`, `FocusSession`, `Notification`, `UserSettings`

### 2. `AhmedOS.Application`
* Encapsulates application-specific use cases and interfaces.
* Coordinates domain entities and dictates contracts for external services.
* Key Interfaces:
  - `IPriorityEngine`: Rules for calculating transparent priority scores and explainable rationale.
  - `IAiService`: Abstraction for AI copilot operations (daily briefing, goal decomposition, chat).
  - `ICalendarProvider`: Abstraction for calendar synchronizations.

### 3. `AhmedOS.Infrastructure`
* Implements interfaces defined in the Application layer.
* Manages persistence via **Entity Framework Core 10** with SQL Server.
* Features:
  - `AhmedOSDbContext`: DbSets, entity configurations, cascading constraints, and soft-delete filters.
  - `DatabaseSeeder`: Rich seed data customized for Ahmed's courses, projects, and roadmaps.
  - `PriorityEngine`: Concrete multi-factor priority algorithm.
  - `LocalAiService`: Heuristic assistant operating locally without mandatory third-party API dependencies.

### 4. `AhmedOS.Web`
* The presentation host combining server-rendered **Razor Pages** and high-speed **REST APIs**.
* Secure cookie-based authentication via **ASP.NET Core Identity**.
* Frontend:
  - Vanilla JavaScript for maximum performance and zero build dependencies.
  - Custom responsive CSS design system (Dark and Light modes, tokenized typography and spacing).
  - Command Palette (⌘K) and Global Search.
  - Kanban Board and Eisenhower Matrix view engines.

---

## Data Flow
1. **User Interaction**: Browser interacts with Razor Page or invokes REST API (`/api/search`, `/api/tasks`, `/api/ai`).
2. **Controller/Page Handler**: Authenticates request, extracts current `UserId`.
3. **Application Services**: Coordinates business logic and calculates priority scoring.
4. **Infrastructure Layer**: Interacts with database through `AhmedOSDbContext` using parameterized queries.
5. **UI Rendering**: Returns reactive response, updates DOM, and gives instant user feedback via Toast notifications.
