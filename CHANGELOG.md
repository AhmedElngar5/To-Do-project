# Changelog

All notable changes to the **Ahmed OS** personal operating system are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-09-13

### Added
- **Clean Architecture Foundation**: 4-layer architecture (.NET 10, EF Core 10, SQL Server).
- **Core Productivity Suite**:
  - Today dashboard with dynamic greeting, time-blocking, and morning briefing.
  - Tasks management with 3 views: List View, Kanban Board, and Eisenhower Matrix.
  - Smart Priority Engine calculating transparent priority scores (0-100) with explainable reasoning.
  - Quick Capture Inbox (GTD workflow).
- **Planning & Calendar**:
  - Monthly interactive calendar with task deadlines.
  - Day and week planning workspace with habit integration.
- **Academic & Learning Systems**:
  - University tracking (Courses, credits, GPA projection, assignment deadlines).
  - DEPI workspace (Ministry training modules, session logs, and attendance).
  - Study tracking (Topics, mastery confidence levels, revision scheduling).
  - Full Stack .NET Developer Roadmap.
  - Pomodoro Focus timer with distraction-free tracking.
- **Graduation Project**:
  - Dedicated **Wasel** workspace with 8 subsystem tracks (AI & Biometrics, NLP, Web Portal, Mobile App, Backend, Database, Docs, QA) and milestone progress tracking.
- **Career & Habits**:
  - Job application pipeline with company tracking and interview stages.
  - Habit streak tracker with weekly grid visualization.
  - Markdown notes with categorization and tagging.
- **Intelligence & Utilities**:
  - In-app Notification Center with badge indicators and click navigation.
  - Global Search API (`/api/search`) and ⌘K Command Palette.
  - AI Assistant drawer (`Ctrl+J`) for recommendations, daily briefings, and automatic goal decomposition.
  - Full JSON backup archive export and CSV task export.
- **Testing & DevOps**:
  - xUnit test suite (`tests/AhmedOS.Tests`) with 100% passing tests.
  - Multi-stage Dockerfile and docker-compose deployment.
  - GitHub Actions CI build & test workflow.
