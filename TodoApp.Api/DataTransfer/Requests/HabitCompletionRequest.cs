namespace TodoApp.Api.DataTransfer.Requests;

public class HabitCompletionRequest
{
    public DateOnly Date { get; set; }
    public bool Completed { get; set; }
}
