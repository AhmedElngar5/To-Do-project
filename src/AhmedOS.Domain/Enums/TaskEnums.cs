namespace AhmedOS.Domain.Enums;

public enum TodoTaskStatus
{
    Inbox = 0,
    Planned = 1,
    Scheduled = 2,
    InProgress = 3,
    Blocked = 4,
    Waiting = 5,
    Completed = 6,
    Cancelled = 7,
    Archived = 8
}

public enum Priority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3
}

public enum EnergyLevel
{
    Low = 0,
    Medium = 1,
    High = 2
}

public enum TaskContext
{
    Computer = 0,
    Study = 1,
    University = 2,
    Home = 3,
    Career = 4,
    Project = 5,
    Phone = 6
}

public enum RecurrenceType
{
    None = 0,
    Daily = 1,
    Weekdays = 2,
    SelectedDays = 3,
    Weekly = 4,
    Monthly = 5,
    Custom = 6
}
