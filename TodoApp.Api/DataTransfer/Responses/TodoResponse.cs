namespace TodoApp.Api.DataTransfer.Responses;

public class TodoResponse
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsCompleted { get; init; }
    public DateTime? DueDate { get; init; }
    public DateTime? StartAt { get; init; }
    public DateTime? EndAt { get; init; }
    public int? ReminderMinutesBefore { get; init; }
    public int? RepeatEveryHours { get; init; }
    public string Category { get; init; } = "Personal";
    public int Priority { get; init; }
    public DateTime CreatedAt { get; init; }
}
