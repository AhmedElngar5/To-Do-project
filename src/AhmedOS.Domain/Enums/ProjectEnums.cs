namespace AhmedOS.Domain.Enums;

public enum ProjectStatus
{
    Planning = 0,
    Active = 1,
    OnHold = 2,
    Completed = 3,
    Cancelled = 4,
    Archived = 5
}

public enum ProjectHealth
{
    Healthy = 0,
    NeedsAttention = 1,
    AtRisk = 2,
    Blocked = 3
}

public enum ProjectType
{
    Personal = 0,
    University = 1,
    DEPI = 2,
    Graduation = 3,
    Learning = 4,
    Portfolio = 5,
    DotNet = 6,
    AI = 7,
    Web = 8,
    Backend = 9,
    Automation = 10,
    Other = 11
}
