namespace AhmedOS.Domain.Enums;

public enum NoteType
{
    Quick = 0,
    Study = 1,
    Project = 2,
    Meeting = 3,
    Career = 4,
    Idea = 5,
    Research = 6
}

public enum NotificationType
{
    TaskReminder = 0,
    Deadline = 1,
    Habit = 2,
    Study = 3,
    Project = 4,
    CareerFollowUp = 5,
    System = 6,
    AISuggestion = 7
}

public enum CalendarEventType
{
    Class = 0,
    Exam = 1,
    Assignment = 2,
    DEPISession = 3,
    Meeting = 4,
    Interview = 5,
    StudySession = 6,
    ProjectMilestone = 7,
    Personal = 8,
    FocusBlock = 9,
    Deadline = 10
}

public enum TimeOfDay
{
    Morning = 0,
    Afternoon = 1,
    Evening = 2
}
