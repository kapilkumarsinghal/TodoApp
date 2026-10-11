namespace TodoApp.Api.DataTransfer.Responses;

public class HabitResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Emoji { get; init; } = "✅";
    public DateTime CreatedAt { get; init; }
    public int[] CompletedDays { get; init; } = [];
}
