namespace TodoApp.Api.DataTransfer.Requests;

public class TodoCreateRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public int? ReminderMinutesBefore { get; set; }
    public int? RepeatEveryHours { get; set; }
    public string Category { get; set; } = "Personal";
    public int Priority { get; set; }
}
