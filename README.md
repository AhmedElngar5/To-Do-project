# 🖥️ Ahmed OS — Personal Operating System

> AI-Powered Life, Study, Career & Productivity Platform

A unified command center for life, university, DEPI training, learning, career, projects, goals, habits, schedule, and long-term development.

## 🏗️ Architecture

Built with **Clean Architecture** using .NET 10:

```
src/
├── AhmedOS.Domain/           # Entities, Enums, Value Objects
├── AhmedOS.Application/      # Interfaces, Service Contracts
├── AhmedOS.Infrastructure/   # EF Core, Database, External Services
└── AhmedOS.Web/              # Razor Pages, API Controllers, UI
```

## ⚡ Tech Stack

| Layer | Technology |
|---|---|
| **Backend** | ASP.NET Core 10, Razor Pages |
| **Database** | SQL Server LocalDB, EF Core 10 |
| **Auth** | ASP.NET Core Identity |
| **Frontend** | Vanilla JavaScript, Custom CSS Design System |
| **AI** | IAiService abstraction (pluggable) |

## 🚀 Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server LocalDB (included with Visual Studio)

### Run
```bash
dotnet build AhmedOS.slnx
dotnet run --project src/AhmedOS.Web
```

Open [http://localhost:5086](http://localhost:5086)

### Default Login
- **Email:** `ahmed@ahmeddos.local`
- **Password:** `Ahmed@OS2024!`

## 📦 Features

### ✅ Core Productivity
- **Dashboard** — Live stats, today's schedule, habits, active projects, goals
- **Today** — Morning briefing, daily focus priorities, time-blocked schedule
- **Tasks** — 3 powerful views:
  - 📋 **List View**: Filtering by Today, Overdue, High Priority, Inbox, Completed
  - 📊 **Kanban Board**: Drag/move workflow across Inbox, Planned, In Progress, Blocked, Done
  - 🎯 **Eisenhower Matrix**: 4-quadrant urgency vs importance decision grid
- **Smart Priority Engine** — Automated dynamic scoring (0-100) with explainable priority reasoning
- **Inbox** — GTD-style quick capture
- **Command Palette & Global Search** — ⌘K instant search across tasks, projects, goals, notes, topics, habits
- **Notifications Center** — In-app alerts, reminders, and unread badges
- **AI Assistant / Copilot** — Context-aware daily briefing, next action advisor, and automatic goal decomposition

### 📋 Planning
- **Planner** — Day/week planner with unscheduled priorities and habits checklist
- **Calendar** — Monthly interactive grid with events and task deadlines

### 🚀 Work
- **Projects** — Milestones, progress tracking, health indicators
- **Goals** — Hierarchical (Vision → Year → Quarter → Month)
- **Habits** — Weekly tracker with streaks

### 📚 Learning
- **Study** — Topic mastery and confidence tracking
- **Skills** — Full .NET developer roadmap
- **University** — Courses, grades, assignments
- **DEPI** — Training modules and attendance
- **Focus** — Pomodoro timer with session history

### 💼 Career
- **Career** — Job application pipeline with status tracking

### 📊 Analytics & Review
- **Analytics** — Productivity stats and metrics
- **Reviews** — Daily/weekly reflection with mood tracking
- **Notes** — Knowledge base with markdown support

## 🔐 Security
- ASP.NET Core Identity with cookie auth
- User-scoped data (multi-tenant ready)
- Soft-delete with global query filters
- HTTPS enforcement in production

## 📐 Domain Model
28 entities covering: Tasks, Projects, Goals, Habits, Notes, Calendar Events, Study Topics, Skills, Courses, DEPI Modules, Career (Companies, Applications, Interviews), Focus Sessions, Time Entries, Notifications, Audit Logs, User Settings, Daily Reviews.

## 📄 License
Private — Built by Ahmed Hany Kamal El Nagar
