namespace AhmedOS.Domain.Enums;

public enum JobApplicationStatus
{
    Saved = 0,
    ReadyToApply = 1,
    Applied = 2,
    Screening = 3,
    TechnicalInterview = 4,
    HRInterview = 5,
    Offer = 6,
    Rejected = 7,
    Withdrawn = 8
}

public enum EmploymentType
{
    FullTime = 0,
    PartTime = 1,
    Contract = 2,
    Freelance = 3,
    Internship = 4,
    Remote = 5
}

public enum InterviewType
{
    Phone = 0,
    Technical = 1,
    HR = 2,
    OnSite = 3,
    TakeHome = 4,
    Panel = 5,
    Other = 6
}

public enum InterviewStatus
{
    Scheduled = 0,
    Completed = 1,
    Cancelled = 2,
    Rescheduled = 3
}
