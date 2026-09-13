# 🔌 REST API Documentation

All API endpoints require authentication via Identity cookie (`[Authorize]`).

---

## 1. Global Search API
### `GET /api/search?q={query}`
Searches across Tasks, Projects, Goals, Notes, Study Topics, and Habits.

**Query Parameters:**
* `q` (string, min length: 2) — Search term.

**Sample Response:**
```json
{
  "results": [
    {
      "id": 1,
      "title": "Build Face Embedding Pipeline",
      "type": "Task",
      "icon": "✅",
      "url": "/Tasks",
      "sub": "InProgress"
    },
    {
      "id": 1,
      "title": "Wasel",
      "type": "Project",
      "icon": "🚀",
      "url": "/Projects",
      "sub": "Active"
    }
  ]
}
```

---

## 2. Notification Center API
### `GET /api/notifications`
Returns the 20 most recent notifications for the authenticated user.

### `GET /api/notifications/unread-count`
Returns the count of unread notifications for badge display.
```json
{
  "count": 3
}
```

### `POST /api/notifications/{id}/read`
Marks a specific notification as read.

### `POST /api/notifications/read-all`
Marks all notifications as read for the user.

---

## 3. AI Assistant & Copilot API
### `POST /api/ai/chat`
Sends a message to the AI copilot assistant.

**Payload:**
```json
{
  "message": "What should I do now?"
}
```

**Response:**
```json
{
  "reply": "Recommended next action: **Build Face Embedding Pipeline** (~45 min) (Urgent deadline • Part of active Project). Start a 25-minute Focus Session to get momentum!"
}
```

### `GET /api/ai/briefing`
Returns an intelligent summary of today's workload and schedule.

### `GET /api/ai/next-action`
Calculates and returns the highest priority actionable recommendation.

### `POST /api/ai/decompose-goal`
Breaks down a high-level goal into actionable milestones and steps.

### `POST /api/ai/recalculate-priorities`
Recalculates transparent priority scores for all tasks of the user.

---

## 4. Data Export & Backup API
### `GET /api/data/export`
Generates and downloads a complete JSON archive of all tasks, projects, goals, habits, notes, and academic data.

### `GET /api/data/export-tasks-csv`
Generates and downloads a CSV spreadsheet of all tasks with their priorities, deadlines, and scores.

---

## 5. Health Check
### `GET /health`
System health check endpoint (Public).
```json
{
  "status": "healthy",
  "timestamp": "2026-09-13T10:00:00Z"
}
```
